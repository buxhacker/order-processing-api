using OrderProcessingAPI.Interfaces;
using OrderProcessingAPI.Models;

namespace OrderProcessingAPI.Services;

public class DiscountProvider : IDiscountProvider
{
    public decimal CalculateDiscount(Order order)
    {
        var itemCount = order.Items.Sum(i => i.Quantity);
        if (itemCount > 3)
        {
            return order.TotalAmount * 0.1m;
        }
        return 0m;
    }
}
