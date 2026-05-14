using Microsoft.EntityFrameworkCore;
using Order.Api.Consumers;
using Order.Api.Data;
using Order.Api.Endpoints;
using Order.Api.Handlers;
using Order.Api.Repositories;
using Order.Api.Services;
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapOrderEndpoints();

app.Run();

