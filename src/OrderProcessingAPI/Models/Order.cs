using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderProcessingAPI.Models;

public class Order
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string CustomerId { get; set; } = default!;

    public List<OrderItem> Items { get; set; } = [];

    [Required]
    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = default!;
}

public class OrderItem
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; } = default!;
    public int Quantity { get; set; }
    public decimal Price { get; set; }

    [ForeignKey("Order")]
    public Guid OrderId { get; set; }
    public Order? Order { get; set; }
}