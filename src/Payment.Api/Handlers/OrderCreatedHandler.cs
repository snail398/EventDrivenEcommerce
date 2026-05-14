using Shared.Contracts.Events;
using Shared.Messaging;

namespace Payment.Api.Handlers;

public sealed class OrderCreatedHandler
{
    private readonly RabbitMqPublisher _publisher;

    public OrderCreatedHandler(RabbitMqPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task HandleAsync(OrderCreatedEvent message, CancellationToken cancellationToken)
    {
        var paymentSucceeded = Random.Shared.Next(1, 101) <= 80;

        if (paymentSucceeded)
        {
            var @event = new PaymentSucceededEvent(message.OrderId);
            await _publisher.PublishAsync("payment.events", "payment.succeeded", @event, cancellationToken);
            Console.WriteLine($"[Payment] Payment succeeded for order {message.OrderId}");
            return;
        }

        var failedEvent = new PaymentFailedEvent(message.OrderId, "Payment was declined.");
        await _publisher.PublishAsync("payment.events", "payment.failed", failedEvent, cancellationToken);
        Console.WriteLine($"[Payment] Payment failed for order {message.OrderId}");
    }
}