using Order.Api.Services;
using Shared.Contracts.Events;

namespace Order.Api.Handlers;

public sealed class PaymentEventHandler
{
    private readonly OrderService _orderService;

    public PaymentEventHandler(OrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task HandleAsync(PaymentSucceededEvent message, CancellationToken cancellationToken)
    {
        await _orderService.ConfirmAsync(message.OrderId, cancellationToken);
        Console.WriteLine($"[Order] Order confirmed: {message.OrderId}");
    }

    public async Task HandleAsync(PaymentFailedEvent message, CancellationToken cancellationToken)
    {
        await _orderService.FailAsync(message.OrderId, cancellationToken);
        Console.WriteLine($"[Order] Order failed: {message.OrderId}. Reason: {message.Reason}");
    }
}