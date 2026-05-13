using Catalog.Api.Data;
using Catalog.Api.Endpoints;
using Catalog.Api.Messaging;
using Catalog.Api.Repositories;
using Catalog.Api.Services;
using Microsoft.EntityFrameworkCore;
using Shared.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.AddSingleton<RabbitMqPublisher>();

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

app.MapProductEndpoints();

app.Run();
