using Notification.Api.Messaging.Configuration;
using RabbitMQ.Client;

namespace Notification.Api.Messaging;

public sealed class RabbitMqTopologyInitializer
{
    public async Task InitializeAsync(IChannel channel, CancellationToken cancellationToken)
    {
        await channel.ExchangeDeclareAsync(RabbitMqTopology.CatalogExchange, ExchangeType.Topic, durable: true, cancellationToken: cancellationToken);
        await channel.ExchangeDeclareAsync(RabbitMqTopology.DeadLetterExchange, ExchangeType.Topic, durable: true, cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(RabbitMqTopology.ProductCreatedDeadLetterQueue, durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
        await channel.QueueBindAsync(RabbitMqTopology.ProductCreatedDeadLetterQueue, RabbitMqTopology.DeadLetterExchange, RabbitMqTopology.DeadLetterRoutingKey, cancellationToken: cancellationToken);

        var retryArguments = new Dictionary<string, object?>
        {
            ["x-dead-letter-exchange"] = RabbitMqTopology.CatalogExchange,
            ["x-dead-letter-routing-key"] = RabbitMqTopology.ProductCreatedRoutingKey,
            ["x-message-ttl"] = RabbitMqTopology.RetryDelayMilliseconds
        };

        await channel.QueueDeclareAsync(RabbitMqTopology.ProductCreatedRetryQueue, durable: true, exclusive: false, autoDelete: false, arguments: retryArguments, cancellationToken: cancellationToken);
        await channel.QueueBindAsync(RabbitMqTopology.ProductCreatedRetryQueue, RabbitMqTopology.CatalogExchange, RabbitMqTopology.RetryRoutingKey, cancellationToken: cancellationToken);

        var mainQueueArguments = new Dictionary<string, object?>
        {
            ["x-dead-letter-exchange"] = RabbitMqTopology.DeadLetterExchange,
            ["x-dead-letter-routing-key"] = RabbitMqTopology.DeadLetterRoutingKey
        };

        await channel.QueueDeclareAsync(RabbitMqTopology.ProductCreatedQueue, durable: true, exclusive: false, autoDelete: false, arguments: mainQueueArguments, cancellationToken: cancellationToken);
        await channel.QueueBindAsync(RabbitMqTopology.ProductCreatedQueue, RabbitMqTopology.CatalogExchange, RabbitMqTopology.ProductCreatedRoutingKey, cancellationToken: cancellationToken);
    }
}