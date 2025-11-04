using OrderProcessingAPI.Models;

namespace OrderProcessingAPI.Interfaces;

public interface IDiscountProvider
{
    decimal CalculateDiscount(Order order);
}