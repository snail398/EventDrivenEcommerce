using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Order.Api.Handlers;
using Order.Api.Messaging.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Contracts.Events;
using Shared.Messaging;

namespace Order.Api.Consumers;

public sealed class PaymentEventConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMqOptions _rabbitMqOptions;

    public PaymentEventConsumer(IServiceScopeFactory scopeFactory, IOptions<RabbitMqOptions> rabbitMqOptions)
    {
        _scopeFactory = scopeFactory;
        _rabbitMqOptions = rabbitMqOptions.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory { HostName = _rabbitMqOptions.HostName, UserName = _rabbitMqOptions.UserName, Password = _rabbitMqOptions.Password };
        var connection = await factory.CreateConnectionAsync(stoppingToken);
        var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.ExchangeDeclareAsync(RabbitMqTopology.PaymentExchange, ExchangeType.Topic, durable: true, cancellationToken: stoppingToken);
        await channel.QueueDeclareAsync(RabbitMqTopology.PaymentEventsQueue, durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);
        await channel.QueueBindAsync(RabbitMqTopology.PaymentEventsQueue, RabbitMqTopology.PaymentExchange, RabbitMqTopology.PaymentSucceededRoutingKey, cancellationToken: stoppingToken);
        await channel.QueueBindAsync(RabbitMqTopology.PaymentEventsQueue, RabbitMqTopology.PaymentExchange, RabbitMqTopology.PaymentFailedRoutingKey, cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, args) =>
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<PaymentEventHandler>();
                var json = Encoding.UTF8.GetString(args.Body.ToArray());

                if (args.RoutingKey == RabbitMqTopology.PaymentSucceededRoutingKey)
                {
                    var message = JsonSerializer.Deserialize<PaymentSucceededEvent>(json) ?? throw new InvalidOperationException("PaymentSucceededEvent deserialization failed.");
                    await handler.HandleAsync(message, stoppingToken);
                }
                else if (args.RoutingKey == RabbitMqTopology.PaymentFailedRoutingKey)
                {
                    var message = JsonSerializer.Deserialize<PaymentFailedEvent>(json) ?? throw new InvalidOperationException("PaymentFailedEvent deserialization failed.");
                    await handler.HandleAsync(message, stoppingToken);
                }

                await channel.BasicAckAsync(args.DeliveryTag, false, stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Order] Payment event error: {ex.Message}");
                await channel.BasicNackAsync(args.DeliveryTag, false, requeue: false, stoppingToken);
            }
        };

        await channel.BasicConsumeAsync(RabbitMqTopology.PaymentEventsQueue, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
    }
}