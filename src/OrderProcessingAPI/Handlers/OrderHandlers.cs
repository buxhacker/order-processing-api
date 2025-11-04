using Microsoft.EntityFrameworkCore;
using OrderProcessingAPI.Commands;
using OrderProcessingAPI.Data;
using OrderProcessingAPI.Enums;
using OrderProcessingAPI.Interfaces;
using OrderProcessingAPI.Validators;
using Wolverine.Attributes;

namespace OrderProcessingAPI.Handlers;

[WolverineHandler]
public class OrderHandlers
{
    private static int _processedOrders = 0;

    public async Task Handle(SubmitOrderCommand cmd, OrderDbContext db, ILogger logger, IDiscountProvider discountProvider)
    {
        var order = await db.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(x => x.Id == cmd.OrderId && x.Status == OrderStatus.New.ToString());

        if (order is null)
            return;

        var validationResult = SubmitOrderValidator.Validate(order);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Order {OrderId} validation failed: {Reason}", order.Id, validationResult.Reason);
            return;
        }

        var discount = discountProvider.CalculateDiscount(order);

        order.TotalAmount -= discount;
        logger.LogInformation("Applied discount of {Discount:C} for order {OrderId}.", discount, order.Id);

        order.Status = OrderStatus.Processed.ToString();

        await db.SaveChangesAsync();

        logger.LogInformation("Processed order {OrderId}.", order.Id);

        var count = Interlocked.Increment(ref _processedOrders);
        logger.LogInformation("Processed order {OrderId}. Total processed: {Count}", order.Id, count);
    }
}