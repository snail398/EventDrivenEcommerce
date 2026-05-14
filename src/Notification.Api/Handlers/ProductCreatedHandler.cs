using Shared.Contracts.Events;

namespace Notification.Api.Handlers;

public sealed class ProductCreatedHandler
{
    private readonly ILogger<ProductCreatedHandler> _logger;

    public ProductCreatedHandler(ILogger<ProductCreatedHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(ProductCreatedEvent message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[Notification] Product created: {Name} - {Price}", message.Name, message.Price);
        return Task.CompletedTask;
    }
}