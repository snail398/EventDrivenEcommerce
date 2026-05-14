using Catalog.Api.Data;
using Catalog.Api.Endpoints;
using Catalog.Api.Repositories;
using Catalog.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Serilog;
using Shared.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration).WriteTo.Console();
});

builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.AddSingleton<RabbitMqPublisher>();

builder.Services.AddDbContext<CatalogDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"));
});
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddOpenApi();
builder.Services.AddScoped<ProductService>();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<CatalogDbContext>()
    .AddRabbitMQ(async serviceProvider =>
    {
        var options = serviceProvider.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
        var factory = new ConnectionFactory { HostName = options.HostName, UserName = options.UserName, Password = options.Password };
        return await factory.CreateConnectionAsync();
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    await dbContext.Database.MigrateAsync();
    await CatalogDbSeeder.SeedAsync(dbContext);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapProductEndpoints();
app.MapHealthChecks("/health");

app.Run();
