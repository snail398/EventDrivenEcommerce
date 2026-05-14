using Microsoft.Extensions.Options;
using Notification.Api.Consumers;
using Notification.Api.Handlers;
using Notification.Api.Messaging;
using RabbitMQ.Client;
using Shared.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.AddOpenApi();
builder.Services.AddSingleton<RabbitMqRetryService>();
builder.Services.AddSingleton<RabbitMqTopologyInitializer>();
builder.Services.AddHostedService<ProductCreatedConsumer>();
builder.Services.AddScoped<ProductCreatedHandler>();

builder.Services.AddHealthChecks()
    .AddRabbitMQ(async serviceProvider =>
    {
        var options = serviceProvider.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
        var factory = new ConnectionFactory { HostName = options.HostName, UserName = options.UserName, Password = options.Password };
        return await factory.CreateConnectionAsync();
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapHealthChecks("/health");

app.Run();


