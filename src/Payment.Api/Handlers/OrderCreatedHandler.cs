using Shared.Contracts.Events;
using Shared.Messaging;

namespace Payment.Api.Handlers;

public sealed class OrderCreatedHandler
{
    private readonly RabbitMqPublisher _publisher;
    private readonly ILogger<OrderCreatedHandler> _logger;

    public OrderCreatedHandler(RabbitMqPublisher publisher, ILogger<OrderCreatedHandler> logger)
    {
        _publisher = publisher;
        _logger = logger;
    }

    public async Task HandleAsync(OrderCreatedEvent message, CancellationToken cancellationToken)
    {
        var paymentSucceeded = Random.Shared.Next(1, 101) <= 80;

        if (paymentSucceeded)
        {
            var @event = new PaymentSucceededEvent(message.OrderId, message.CorrelationId);
            await _publisher.PublishAsync("payment.events", "payment.succeeded", @event, cancellationToken);
            _logger.LogInformation("Payment succeeded for order {OrderId}", message.OrderId);
            return;
        }

        var failedEvent = new PaymentFailedEvent(message.OrderId, "Payment was declined.", message.CorrelationId);
        await _publisher.PublishAsync("payment.events", "payment.failed", failedEvent, cancellationToken);
        _logger.LogInformation("Payment failed for order {OrderId}", message.OrderId);
    }
}