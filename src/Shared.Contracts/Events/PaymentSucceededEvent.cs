namespace Shared.Contracts.Events;

public sealed record PaymentSucceededEvent(Guid OrderId, string CorrelationId);