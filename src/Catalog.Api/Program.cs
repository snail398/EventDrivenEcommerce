using Catalog.Api.Data;
using Catalog.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var string1 = builder.Configuration.GetConnectionString("Postgres");
builder.Services.AddDbContext<CatalogDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"));
});

builder.Services.AddOpenApi();
builder.Services.AddSingleton<ProductService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var apiV1 = app.MapGroup("/api/v1");

apiV1.MapGet("/products", (ProductService service) =>
{
    return Results.Ok(service.GetAll());
});

apiV1.MapGet("/products/{id:guid}", (Guid id, ProductService service) =>
{
    var product = service.GetById(id);

    return product is null ? Results.NotFound() : Results.Ok(product);
});

app.Run();
