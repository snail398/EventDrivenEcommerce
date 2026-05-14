using Order.Api.Models;

namespace Order.Api.Repositories;

public interface IOrderRepository
{
    Task<CustomerOrder> AddAsync(CustomerOrder order, CancellationToken cancellationToken);
}