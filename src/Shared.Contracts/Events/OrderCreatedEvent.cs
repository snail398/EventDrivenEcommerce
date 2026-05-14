namespace Shared.Contracts.Events;

public sealed record OrderCreatedEvent(Guid OrderId, Guid ProductId, int Quantity, decimal UnitPrice);