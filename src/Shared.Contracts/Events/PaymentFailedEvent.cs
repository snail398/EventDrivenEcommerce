namespace Shared.Contracts.Events;

public sealed record PaymentFailedEvent(Guid OrderId, string Reason);