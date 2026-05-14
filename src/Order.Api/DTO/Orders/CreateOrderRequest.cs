namespace Order.Api.DTO.Orders;

public sealed record CreateOrderRequest(Guid ProductId, int Quantity, decimal UnitPrice);