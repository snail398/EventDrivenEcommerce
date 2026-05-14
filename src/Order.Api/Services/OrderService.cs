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
        var order = new CustomerOrder(Guid.NewGuid(), request.ProductId, request.Quantity, request.UnitPrice, "Created");
        await _orderRepository.AddAsync(order, cancellationToken);

        var @event = new OrderCreatedEvent(order.Id, order.ProductId, order.Quantity, order.UnitPrice);
        await _publisher.PublishAsync("order.events", "order.created", @event, cancellationToken);
        return order;
    }
}