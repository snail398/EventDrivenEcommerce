using Notification.Api.Messaging.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Notification.Api.Messaging;

public sealed class RabbitMqRetryService
{
    public async Task HandleFailureAsync(IChannel channel, BasicDeliverEventArgs args, CancellationToken cancellationToken)
    {
        var retryCount = GetRetryCount(args);

        if (retryCount >= RabbitMqTopology.MaxRetryCount)
        {
            await channel.BasicPublishAsync(exchange: RabbitMqTopology.DeadLetterExchange, routingKey: RabbitMqTopology.DeadLetterRoutingKey, body: args.Body, cancellationToken: cancellationToken);
            await channel.BasicAckAsync(args.DeliveryTag, false, cancellationToken);
            return;
        }

        var properties = new BasicProperties
        {
            Headers = new Dictionary<string, object?>
            {
                ["x-retry-count"] = retryCount + 1
            }
        };

        await channel.BasicPublishAsync(exchange: RabbitMqTopology.CatalogExchange, routingKey: RabbitMqTopology.RetryRoutingKey, mandatory: false, basicProperties: properties, body: args.Body, cancellationToken: cancellationToken);
        await channel.BasicAckAsync(args.DeliveryTag, false, cancellationToken);
    }

    private static int GetRetryCount(BasicDeliverEventArgs args)
    {
        if (args.BasicProperties.Headers is null || !args.BasicProperties.Headers.TryGetValue("x-retry-count", out var value))
        {
            return 0;
        }

        return Convert.ToInt32(value);
    }
}