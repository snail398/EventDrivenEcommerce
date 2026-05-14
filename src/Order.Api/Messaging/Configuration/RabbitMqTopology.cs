namespace Order.Api.Messaging.Configuration;

public static class RabbitMqTopology
{
    public const string PaymentExchange = "payment.events";
    public const string PaymentSucceededRoutingKey = "payment.succeeded";
    public const string PaymentFailedRoutingKey = "payment.failed";
    public const string PaymentEventsQueue = "order.payment-events";
}