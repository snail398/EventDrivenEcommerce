using Order.Api.Data;
using Order.Api.DTO.Orders;
using Order.Api.Models;
using Order.Api.Services;
using Microsoft.EntityFrameworkCore;
using Order.Api.Middleware;

namespace Order.Api.Endpoints;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/orders");

        group.MapPost("/", async (CreateOrderRequest request, HttpContext httpContext, OrderService service, CancellationToken cancellationToken) =>
        {
            var correlationId = httpContext.Items[CorrelationIdMiddleware.HeaderName]?.ToString() ?? Guid.NewGuid().ToString();
            var order = await service.CreateAsync(request, correlationId, cancellationToken);
            return Results.Created($"/api/v1/orders/{order.Id}", ToResponse(order));
        });

        group.MapGet("/{id:guid}", async (Guid id, OrderService service, CancellationToken cancellationToken) =>
        {
            var order = await service.GetByIdAsync(id, cancellationToken);
            return order is null ? Results.NotFound() : Results.Ok(ToResponse(order));
        });
    
        group.MapGet("/debug/outbox", async (OrderDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var messages = await dbContext.OutboxMessages.OrderByDescending(x => x.OccurredOnUtc).Take(50).ToListAsync(cancellationToken);
            return Results.Ok(messages);
        });

        return app;
    }

    private static OrderResponse ToResponse(CustomerOrder order)
    {
        return new OrderResponse(
            order.Id,
            order.ProductId,
            order.Quantity,
            order.UnitPrice,
            order.TotalPrice,
            order.Status.ToString(),
            order.CreatedOnUtc,
            order.UpdatedOnUtc);
    }
}