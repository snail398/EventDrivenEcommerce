using Order.Api.DTO.Orders;
using Order.Api.Models;
using Order.Api.Repositories;

namespace Order.Api.Services;

public sealed class OrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<CustomerOrder> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var order = new CustomerOrder(Guid.NewGuid(), request.ProductId, request.Quantity, request.UnitPrice, "Created");
        return await _orderRepository.AddAsync(order, cancellationToken);
    }
}