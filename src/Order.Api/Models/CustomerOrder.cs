namespace Order.Api.Models;

public sealed class CustomerOrder
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public OrderStatus Status { get; set; }
}