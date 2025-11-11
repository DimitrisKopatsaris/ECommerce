namespace ECommerce.Api.Dtos;

public sealed class ProductListItemDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public decimal Price { get; init; }
}
