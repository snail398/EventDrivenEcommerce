using System.Text.Json;
using Order.Api.DTO.Orders;
using Order.Api.Exceptions;
using Order.Api.Models;
using Order.Api.Repositories;
using Shared.Contracts.Events;

namespace Order.Api.Services;

public sealed class OrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<CustomerOrder> CreateAsync(CreateOrderRequest request, string correlationId, string? idempotencyKey, string requestHash, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(idempotencyKey))
        {
            var existingRecord = await _orderRepository.GetIdempotencyRecordAsync(idempotencyKey, cancellationToken);

            if (existingRecord is not null)
            {
                if (existingRecord.RequestHash != requestHash)
                {
                    throw new IdempotencyConflictException();
                }

                var existingOrder = JsonSerializer.Deserialize<CustomerOrder>(existingRecord.ResponseBody);

                if (existingOrder is null)
                {
                    throw new InvalidOperationException("Stored idempotency response could not be deserialized.");
                }

                return existingOrder;
            }
        }

        var order = new CustomerOrder
        {
            Id = Guid.NewGuid(),
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            UnitPrice = request.UnitPrice,
            TotalPrice = request.Quantity * request.UnitPrice,
            CreatedOnUtc = DateTime.UtcNow,
            Status = OrderStatus.Created
        };

        var @event = new OrderCreatedEvent(order.Id, order.ProductId, order.Quantity, order.UnitPrice, correlationId);

        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = nameof(OrderCreatedEvent),
            Content = JsonSerializer.Serialize(@event),
            Exchange = "order.events",
            RoutingKey = "order.created",
            OccurredOnUtc = DateTime.UtcNow
        };

        IdempotencyRecord? idempotencyRecord = null;

        if (!string.IsNullOrWhiteSpace(idempotencyKey))
        {
            idempotencyRecord = new IdempotencyRecord
            {
                Id = Guid.NewGuid(),
                Key = idempotencyKey,
                RequestHash = requestHash,
                ResponseBody = JsonSerializer.Serialize(order),
                StatusCode = StatusCodes.Status201Created,
                CreatedOnUtc = DateTime.UtcNow
            };
        }

        await _orderRepository.AddWithOutboxMessageAsync(order, outboxMessage, idempotencyRecord, cancellationToken);
        return order;
    }

    public async Task ConfirmAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order is null) return;

        if (order.Status is OrderStatus.Confirmed or OrderStatus.Failed)
        {
            return;
        }

        order.UpdatedOnUtc = DateTime.UtcNow;
        order.Status = OrderStatus.Confirmed;
        await _orderRepository.UpdateAsync(order, cancellationToken);
    }

    public async Task FailAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order is null) return;

        if (order.Status is OrderStatus.Confirmed or OrderStatus.Failed)
        {
            return;
        }
        order.UpdatedOnUtc = DateTime.UtcNow;
        order.Status = OrderStatus.Failed;
        await _orderRepository.UpdateAsync(order, cancellationToken);
    }

    public Task<CustomerOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _orderRepository.GetByIdAsync(id, cancellationToken);
    }
}