using Order.Api.Data;
using Order.Api.Models;

namespace Order.Api.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _dbContext;

    public OrderRepository(OrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CustomerOrder> AddAsync(CustomerOrder order, CancellationToken cancellationToken)
    {
        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return order;
    }
}