namespace OrderProcessingAPI.Models;

public class SubmitOrderRequest
{
    public string CustomerId { get; set; } = default!;
    public List<OrderItemDto> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
}

public class OrderItemDto
{
    public string Name { get; set; } = default!;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}