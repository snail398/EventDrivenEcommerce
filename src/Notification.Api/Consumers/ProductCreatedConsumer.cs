using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Notification.Api.Handlers;
using Notification.Api.Messaging;
using Notification.Api.Messaging.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Contracts.Events;
using Shared.Messaging;

namespace Notification.Api.Consumers;

public sealed class ProductCreatedConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMqRetryService _retryService;
    private readonly RabbitMqOptions _rabbitMqOptions;
    private readonly RabbitMqTopologyInitializer _topologyInitializer;
    private readonly ILogger<ProductCreatedConsumer> _logger;

    public ProductCreatedConsumer(IServiceScopeFactory scopeFactory, RabbitMqRetryService retryService, IOptions<RabbitMqOptions> rabbitMqOptions, RabbitMqTopologyInitializer topologyInitializer, ILogger<ProductCreatedConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _retryService = retryService;
        _topologyInitializer = topologyInitializer;
        _rabbitMqOptions = rabbitMqOptions.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory { HostName = _rabbitMqOptions.HostName, UserName = _rabbitMqOptions.UserName, Password = _rabbitMqOptions.Password };

        var connection = await factory.CreateConnectionAsync(stoppingToken);
        var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);
        await _topologyInitializer.InitializeAsync(channel, stoppingToken);
        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (sender, args) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(args.Body.ToArray());
                var message = JsonSerializer.Deserialize<ProductCreatedEvent>(json);

                using var scope = _scopeFactory.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<ProductCreatedHandler>();

                if (message is null)
                {
                    throw new InvalidOperationException("ProductCreatedEvent deserialization failed.");
                }

                await handler.HandleAsync(message, stoppingToken);
                await channel.BasicAckAsync(args.DeliveryTag, false, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Notification] Error: {Message}", ex.Message);
                await _retryService.HandleFailureAsync(channel, args, stoppingToken); 
            }
        };

        await channel.BasicConsumeAsync(RabbitMqTopology.ProductCreatedQueue, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
    }
}