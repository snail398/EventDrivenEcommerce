namespace Payment.Api.Messaging.Configuration;

public static class RabbitMqTopology
{
    public const string OrderExchange = "order.events";
    public const string OrderCreatedRoutingKey = "order.created";
    public const string OrderCreatedQueue = "payment.order-created";
}