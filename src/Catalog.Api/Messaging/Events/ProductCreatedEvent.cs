namespace Catalog.Api.Messaging.Events;

public sealed record ProductCreatedEvent(Guid Id, string Name, decimal Price);