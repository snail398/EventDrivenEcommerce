using Microsoft.EntityFrameworkCore;
using Order.Api.Data;
using Shared.Messaging;

namespace Order.Api.Workers;

public sealed class OutboxProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMqPublisher _publisher;

    public OutboxProcessor(IServiceScopeFactory scopeFactory, RabbitMqPublisher publisher)
    {
        _scopeFactory = scopeFactory;
        _publisher = publisher;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

            var messages = await dbContext.OutboxMessages
                .Where(x => x.ProcessedOnUtc == null && x.RetryCount < 3)
                .OrderBy(x => x.OccurredOnUtc)
                .Take(10)
                .ToListAsync(stoppingToken);

            foreach (var message in messages)
            {
               try
                {
                    await _publisher.PublishRawAsync(message.Exchange, message.RoutingKey, message.Content, stoppingToken);
                    message.ProcessedOnUtc = DateTime.UtcNow;
                    message.Error = null;
                }
                catch (Exception ex)
                {
                    message.RetryCount++;
                    message.Error = ex.Message;
                }
            }

            await dbContext.SaveChangesAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}