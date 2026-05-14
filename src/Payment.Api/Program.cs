using Payment.Api.Consumers;
using Payment.Api.Handlers;
using Shared.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.AddSingleton<RabbitMqPublisher>();

builder.Services.AddScoped<OrderCreatedHandler>();
builder.Services.AddHostedService<OrderCreatedConsumer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
