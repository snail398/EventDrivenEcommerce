using Order.Api.DTO.Orders;
using Order.Api.Models;
using Order.Api.Repositories;
using Shared.Contracts.Events;
using Shared.Messaging;

namespace Order.Api.Services;

public sealed class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly RabbitMqPublisher _publisher;

    public OrderService(IOrderRepository orderRepository, RabbitMqPublisher publisher)
    {
        _orderRepository = orderRepository;
        _publisher = publisher; 
    }

    public async Task<CustomerOrder> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var order = new CustomerOrder
        {
            Id = Guid.NewGuid(),
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            UnitPrice = request.UnitPrice,
            Status = OrderStatus.Created
        };
        await _orderRepository.AddAsync(order, cancellationToken);

        var @event = new OrderCreatedEvent(order.Id, order.ProductId, order.Quantity, order.UnitPrice);
        await _publisher.PublishAsync("order.events", "order.created", @event, cancellationToken);
        return order;
    }

    public async Task ConfirmAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order is null) return;

        order.Status = OrderStatus.Confirmed;
        await _orderRepository.UpdateAsync(order, cancellationToken);
    }

    public async Task FailAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order is null) return;

        order.Status = OrderStatus.Failed;
        await _orderRepository.UpdateAsync(order, cancellationToken);
    }

    public Task<CustomerOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _orderRepository.GetByIdAsync(id, cancellationToken);
    }
}