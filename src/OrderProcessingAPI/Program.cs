using Microsoft.EntityFrameworkCore;
using OrderProcessingAPI.Commands;
using OrderProcessingAPI.Data;
using OrderProcessingAPI.Enums;
using OrderProcessingAPI.Handlers;
using OrderProcessingAPI.Interfaces;
using OrderProcessingAPI.Models;
using OrderProcessingAPI.Services;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IDiscountProvider, DiscountProvider>();

builder.Host.UseWolverine(opts =>
{
    opts.UseEntityFrameworkCoreTransactions();
    opts.UseRabbitMq(builder.Configuration["RabbitMQ:Uri"]!)
        .AutoProvision();
    opts.PublishMessage<SubmitOrderCommand>().ToRabbitQueue("orders");
    opts.ListenToRabbitQueue("orders");
            opts.ApplicationAssembly = typeof(Program).Assembly;
    Console.WriteLine(opts.DescribeHandlerMatch(typeof(OrderHandlers)));

});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapPost("/api/orders", async (
    SubmitOrderRequest request,
    OrderDbContext db,
    IMessageBus bus) =>
{
    var order = new Order
    {
        CustomerId = request.CustomerId,
        TotalAmount = request.TotalAmount,
        Status = OrderStatus.New.ToString()
    };
    order.Items = [.. request.Items.Select(i => new OrderItem
    {
        Name = i.Name,
        Quantity = i.Quantity,
        Price = i.Price,
        OrderId = order.Id
    })];

    db.Orders.Add(order);
    await db.SaveChangesAsync();

    await bus.PublishAsync(new SubmitOrderCommand(order.Id));

    return Results.Accepted($"/api/orders/{order.Id}", new { order.Id });
})
.WithName("Orders")
.WithOpenApi();

app.Run();
