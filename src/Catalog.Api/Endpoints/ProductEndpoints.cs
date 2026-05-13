using Catalog.Api.DTO.Products;
using Catalog.Api.Services;

namespace Catalog.Api.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/products");

        group.MapGet("/", async (ProductService service, CancellationToken cancellationToken) =>
        {
            var products = await service.GetAllAsync(cancellationToken);
            return Results.Ok(products);
        });

        group.MapGet("/{id:guid}", async (Guid id, ProductService service, CancellationToken cancellationToken) =>
        {
            var product = await service.GetByIdAsync(id, cancellationToken);
            return product is null ? Results.NotFound() : Results.Ok(product);
        });

        group.MapPost("/", async (CreateProductRequest request, ProductService service, CancellationToken cancellationToken) =>
        {
            var product = await service.CreateAsync(request, cancellationToken);
            return Results.Created($"/api/v1/products/{product.Id}", product);
        });

        group.MapPut("/{id:guid}", async (Guid id, UpdateProductRequest request, ProductService service, CancellationToken cancellationToken) =>
        {
            var updated = await service.UpdateAsync(id, request, cancellationToken);
            return updated ? Results.NoContent() : Results.NotFound();
        });

        group.MapDelete("/{id:guid}", async (Guid id, ProductService service, CancellationToken cancellationToken) =>
        {
            var deleted = await service.DeleteAsync(id, cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        });

        return app;
    }
}