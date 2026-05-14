namespace Order.Api.DTO.Orders;

public sealed record OrderResponse(
    Guid Id,
    Guid ProductId,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice,
    string Status,
    DateTime CreatedOnUtc,
    DateTime? UpdatedOnUtc);