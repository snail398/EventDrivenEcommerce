using Catalog.Api.Data;
using Catalog.Api.DTO.Products;
using Catalog.Api.Repositories;
using Catalog.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CatalogDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"));
});
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddOpenApi();
builder.Services.AddScoped<ProductService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    await CatalogDbSeeder.SeedAsync(dbContext);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var apiV1 = app.MapGroup("/api/v1");

apiV1.MapGet("/products", async (ProductService service, CancellationToken cancellationToken) =>
{
    var products = await service.GetAllAsync(cancellationToken);
    return Results.Ok(products);
});

apiV1.MapGet("/products/{id:guid}", async (Guid id, ProductService service, CancellationToken cancellationToken) =>
{
    var product = await service.GetByIdAsync(id, cancellationToken);

    return product is null ? Results.NotFound() : Results.Ok(product);
});

apiV1.MapPost("/products", async (CreateProductRequest request, ProductService service, CancellationToken cancellationToken) =>
{
    var product = await service.CreateAsync(request, cancellationToken);
    return Results.Created($"/api/v1/products/{product.Id}", product);
});

apiV1.MapPut("/products/{id:guid}", async (Guid id, UpdateProductRequest request, ProductService service, CancellationToken cancellationToken) =>
{
    var updated = await service.UpdateAsync(id, request, cancellationToken);
    return updated ? Results.NoContent() : Results.NotFound();
});

apiV1.MapDelete("/products/{id:guid}", async (Guid id, ProductService service, CancellationToken cancellationToken) =>
{
    var deleted = await service.DeleteAsync(id, cancellationToken);
    return deleted ? Results.NoContent() : Results.NotFound();
});

app.Run();
