using Order.Api.Models;

namespace Order.Api.Repositories;

public interface IOrderRepository
{
    Task<CustomerOrder> AddAsync(CustomerOrder order, CancellationToken cancellationToken);
    Task AddWithOutboxMessageAsync(CustomerOrder order, OutboxMessage outboxMessage, CancellationToken cancellationToken);
    Task<CustomerOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateAsync(CustomerOrder order, CancellationToken cancellationToken);
}