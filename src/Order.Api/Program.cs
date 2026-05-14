using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Order.Api.Consumers;
using Order.Api.Data;
using Order.Api.Endpoints;
using Order.Api.Handlers;
using Order.Api.Repositories;
using Order.Api.Services;
using Order.Api.Workers;
using RabbitMQ.Client;
using Shared.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OrderDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddOpenApi();

builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.AddSingleton<RabbitMqPublisher>();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<PaymentEventHandler>();

builder.Services.AddHostedService<PaymentEventConsumer>();
builder.Services.AddHostedService<OutboxProcessor>();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<OrderDbContext>()
    .AddRabbitMQ(async serviceProvider =>
    {
        var options = serviceProvider.GetRequiredService<IOptions<RabbitMqOptions>>().Value;

        var factory = new ConnectionFactory
        {
            HostName = options.HostName,
            UserName = options.UserName,
            Password = options.Password
        };

        return await factory.CreateConnectionAsync();
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    await dbContext.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapOrderEndpoints();
app.MapHealthChecks("/health");

app.Run();

