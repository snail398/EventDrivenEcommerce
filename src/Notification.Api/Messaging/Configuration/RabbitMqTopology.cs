namespace Notification.Api.Messaging.Configuration;

public static class RabbitMqTopology
{
    public const string CatalogExchange = "catalog.events";
    public const string RetryRoutingKey = "product.created.retry";
    public const string ProductCreatedRoutingKey = "product.created";
    public const string ProductCreatedQueue = "notification.product-created";
    public const string ProductCreatedRetryQueue = "notification.product-created.retry";
    public const string DeadLetterExchange = "notification.dead-letter";
    public const string DeadLetterRoutingKey = "product.created.failed";
    public const string ProductCreatedDeadLetterQueue = "notification.product-created.dlq";
    public const int RetryDelayMilliseconds = 5000;
    public const int MaxRetryCount = 2;
}