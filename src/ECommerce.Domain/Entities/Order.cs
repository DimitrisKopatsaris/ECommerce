namespace ECommerce.Domain.Entities;

public class Order
{
    public int Id { get; set; }

    // ✅ relationship to Customer
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = default!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // replaces CreatedAtUtc
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal Total { get; set; }                          // replaces TotalAmount

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
