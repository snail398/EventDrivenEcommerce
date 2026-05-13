namespace Shared.Contracts.Events;

public sealed record ProductCreatedEvent(Guid Id, string Name, decimal Price);