using Order.Api.Services;
using Shared.Contracts.Events;

namespace Order.Api.Handlers;

public sealed class PaymentEventHandler
{
    private readonly OrderService _orderService;
    private readonly ILogger<PaymentEventHandler> _logger;

    public PaymentEventHandler(OrderService orderService, ILogger<PaymentEventHandler> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    public async Task HandleAsync(PaymentSucceededEvent message, CancellationToken cancellationToken)
    {
        await _orderService.ConfirmAsync(message.OrderId, cancellationToken);
        _logger.LogInformation("Order confirmed: {OrderId}. CorrelationId: {CorrelationId}", message.OrderId, message.CorrelationId);
    }

    public async Task HandleAsync(PaymentFailedEvent message, CancellationToken cancellationToken)
    {
        await _orderService.FailAsync(message.OrderId, cancellationToken);
        _logger.LogInformation("Order failed: {OrderId}. Reason: {Reason}. CorrelationId: {CorrelationId}", message.OrderId, message.Reason, message.CorrelationId);
    }
}