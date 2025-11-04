using OrderProcessingAPI.Models;

namespace OrderProcessingAPI.Validators;

public class ValidationResult
{
    public bool IsValid { get; set; }
    public string Reason { get; set; } = string.Empty;

    public static ValidationResult Success() => new() { IsValid = true };
    public static ValidationResult Fail(string reason) => new() { IsValid = false, Reason = reason };
}

public class SubmitOrderValidator
{
    public static ValidationResult Validate(Order order)
    {
        if (order.TotalAmount <= 0)
            return ValidationResult.Fail($"Invalid TotalAmount: {order.TotalAmount}");

        if (order.Items == null || order.Items.Count == 0)
        {
            return ValidationResult.Fail("No items specified.");
        }

        foreach (var item in order.Items)
        {
            if (item.Quantity <= 0 || item.Price < 0 || string.IsNullOrWhiteSpace(item.Name))
                return ValidationResult.Fail($"Invalid item {item.Name} in order.");
        }

        return ValidationResult.Success();
    }
}
