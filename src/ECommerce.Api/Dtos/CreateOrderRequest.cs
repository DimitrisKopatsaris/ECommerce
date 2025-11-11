namespace ECommerce.Api.Dtos;

public sealed class CreateOrderItemDto
{
    public int ProductId { get; set; } // what product does the customer want
    public int Quantity { get; set; } // how many products the customer wants
}

public sealed class CreateOrderRequest
{
    public int CustomerId { get; set; } //who places the order, which customer?
    public List<CreateOrderItemDto> Items { get; set; } = new(); //a list containing his order procucts
}
