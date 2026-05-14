namespace Order.Api.Models;

public sealed record CustomerOrder(Guid Id, Guid ProductId, int Quantity, decimal UnitPrice, string Status);