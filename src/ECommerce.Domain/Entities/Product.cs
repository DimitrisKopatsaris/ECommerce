namespace ECommerce.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }                          // ✅ used in DbContext
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // reverse navigation to OrderItems (optional)
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
