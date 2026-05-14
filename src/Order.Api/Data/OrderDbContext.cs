using Microsoft.EntityFrameworkCore;
using Order.Api.Models;

namespace Order.Api.Data;

public sealed class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

    public DbSet<CustomerOrder> Orders => Set<CustomerOrder>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
}