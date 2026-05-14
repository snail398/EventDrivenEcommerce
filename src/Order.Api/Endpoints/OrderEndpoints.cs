using Order.Api.DTO.Orders;
using Order.Api.Services;

namespace Order.Api.Endpoints;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/orders");

        group.MapPost("/", async (CreateOrderRequest request, OrderService service, CancellationToken cancellationToken) =>
        {
            var order = await service.CreateAsync(request, cancellationToken);
            return Results.Created($"/api/v1/orders/{order.Id}", order);
        });

        return app;
    }
}