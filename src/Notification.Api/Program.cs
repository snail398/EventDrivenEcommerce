using Notification.Api.Consumers;
using Notification.Api.Handlers;
using Notification.Api.Messaging;
using Shared.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.AddOpenApi();
builder.Services.AddSingleton<RabbitMqRetryService>();
builder.Services.AddSingleton<RabbitMqTopologyInitializer>();
builder.Services.AddHostedService<ProductCreatedConsumer>();
builder.Services.AddScoped<ProductCreatedHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();


