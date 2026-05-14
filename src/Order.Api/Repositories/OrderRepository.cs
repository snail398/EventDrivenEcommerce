using Microsoft.EntityFrameworkCore;
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

    public async Task AddWithOutboxMessageAsync(CustomerOrder order, OutboxMessage outboxMessage, CancellationToken cancellationToken)
    {
        _dbContext.Orders.Add(order);
        _dbContext.OutboxMessages.Add(outboxMessage);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<CustomerOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Orders.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(CustomerOrder order, CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}