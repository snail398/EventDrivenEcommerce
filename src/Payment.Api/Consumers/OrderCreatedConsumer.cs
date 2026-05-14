using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Payment.Api.Handlers;
using Payment.Api.Messaging.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog.Context;
using Shared.Contracts.Events;
using Shared.Messaging;

namespace Payment.Api.Consumers;

public sealed class OrderCreatedConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMqOptions _rabbitMqOptions;
    private readonly ILogger<OrderCreatedConsumer> _logger;

    public OrderCreatedConsumer(IServiceScopeFactory scopeFactory, IOptions<RabbitMqOptions> rabbitMqOptions, ILogger<OrderCreatedConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _rabbitMqOptions = rabbitMqOptions.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory { HostName = _rabbitMqOptions.HostName, UserName = _rabbitMqOptions.UserName, Password = _rabbitMqOptions.Password };
        var connection = await factory.CreateConnectionAsync(stoppingToken);
        var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.ExchangeDeclareAsync(RabbitMqTopology.OrderExchange, ExchangeType.Topic, durable: true, cancellationToken: stoppingToken);
        await channel.QueueDeclareAsync(RabbitMqTopology.OrderCreatedQueue, durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);
        await channel.QueueBindAsync(RabbitMqTopology.OrderCreatedQueue, RabbitMqTopology.OrderExchange, RabbitMqTopology.OrderCreatedRoutingKey, cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, args) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(args.Body.ToArray());
                var message = JsonSerializer.Deserialize<OrderCreatedEvent>(json);

                if (message is null) throw new InvalidOperationException("OrderCreatedEvent deserialization failed.");

                using (LogContext.PushProperty("CorrelationId", message.CorrelationId))
                {
                    using var scope = _scopeFactory.CreateScope();
                    var handler = scope.ServiceProvider.GetRequiredService<OrderCreatedHandler>();

                    await handler.HandleAsync(message, stoppingToken);
                    await channel.BasicAckAsync(args.DeliveryTag, false, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[Payment] Error: {ex.Message}");
                await channel.BasicNackAsync(args.DeliveryTag, false, requeue: true, stoppingToken);
            }
        };

        await channel.BasicConsumeAsync(RabbitMqTopology.OrderCreatedQueue, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
    }
}