namespace ECommerce.Api.Dtos;

public sealed class OrderItemDto
{
    public int ProductId { get; init; } // init is a special kind of property that means: the property can only be set once, when the object is being created (initialized) but not changed later. It’s like a “set-only-during-initialization” version of set.
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
}

public sealed class OrderDto
{
    public int Id { get; init; }
    public string Status { get; init; } = string.Empty;
    public decimal Total { get; init; }
    public DateTime CreatedAt { get; init; }
    public IReadOnlyList<OrderItemDto> Items { get; init; } = Array.Empty<OrderItemDto>();
}
