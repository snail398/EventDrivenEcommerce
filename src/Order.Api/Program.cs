using Microsoft.EntityFrameworkCore;
using Order.Api.Data;
using Order.Api.Endpoints;
using Order.Api.Repositories;
using Order.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OrderDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddOpenApi();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<OrderService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapOrderEndpoints();

app.Run();

