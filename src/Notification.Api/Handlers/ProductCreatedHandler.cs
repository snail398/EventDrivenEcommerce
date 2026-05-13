using Shared.Contracts.Events;

namespace Notification.Api.Handlers;

public sealed class ProductCreatedHandler
{
    public Task HandleAsync(ProductCreatedEvent message, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[Notification] Product created: {message.Name} - {message.Price}");
        return Task.CompletedTask;
    }
}