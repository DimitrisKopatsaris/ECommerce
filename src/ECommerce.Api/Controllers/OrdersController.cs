using ECommerce.Api.Dtos;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure; //db context
using ECommerce.Api.Models;          // only in ProductsController
using ECommerce.Api.Extensions;      // only in ProductsController
using Microsoft.AspNetCore.Mvc; 
using Microsoft.EntityFrameworkCore;


namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ECommerceDbContext _db;
    public OrdersController(ECommerceDbContext db) => _db = db; //classic DI , _db is my live connection to the SQL database

    // Request DTOs (simple & explicit)
    public record CreateOrderItemRequest(int ProductId, int Quantity);
    public record CreateOrderRequest(int CustomerId, List<CreateOrderItemRequest> Items);

    // POST: /api/orders
    // Creates an order, sets UnitPrice from Product.Price, computes TotalAmount
    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderRequest req, CancellationToken ct)
    {
        // ModelState is handled by FluentValidation automatically

        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        // Load all product rows we need and lock them for update
        var productIds = req.Items.Select(i => i.ProductId).ToList();
        var products = await _db.Products
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync(ct);

        // Validate existence + stock
        foreach (var line in req.Items)
        {
            var product = products.FirstOrDefault(p => p.Id == line.ProductId);
            if (product is null)
                return NotFound($"Product {line.ProductId} not found.");

            if (product.Stock < line.Quantity)
                return BadRequest($"Insufficient stock for product {product.Name} (have {product.Stock}, need {line.Quantity}).");
        }

        // Create order + decrement stock
        var order = new Order
        {
            CustomerId = req.CustomerId,
            CreatedAt = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            Items = req.Items.Select(i =>
            {
                var prod = products.First(p => p.Id == i.ProductId);
                prod.Stock -= i.Quantity; // decrement
                return new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = prod.Price
                };
            }).ToList()
        };

        order.Total = order.Items.Sum(i => i.UnitPrice * i.Quantity);

        _db.Orders.Add(order);
        await _db.SaveChangesAsync(ct); // until here the changes are logged but not yet permanently commited-saved to the database.

        await tx.CommitAsync(ct); // all the operations done since BeginTransaction were succesfull, now make them permanent and continue.

        var dto = new OrderDto
        {
            Id = order.Id,
            Status = order.Status.ToString(),
            Total = order.Total,
            CreatedAt = order.CreatedAt,
            Items = order.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        return CreatedAtAction(nameof(GetById), new { id = order.Id }, dto);
    }


    // GET: /api/orders
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetAll(CancellationToken ct)
    {
        var data = await _db.Orders
            .AsNoTracking()
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new
            {
                o.Id,
                o.CustomerId,
                o.CreatedAt,
                o.Total,
                ItemsCount = o.Items.Count
            })
            .ToListAsync(ct);

        return Ok(data);
    }

    // GET: /api/orders/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<object>> GetById(int id, CancellationToken ct)
    {
        var order = await _db.Orders
            .AsNoTracking()
            .Where(o => o.Id == id)
            .Select(o => new
            {
                o.Id,
                o.CustomerId,
                o.CreatedAt,
                o.Total,
                Items = o.Items.Select(oi => new
                {
                    oi.Id,
                    oi.ProductId,
                    oi.Quantity,
                    oi.UnitPrice,
                    LineTotal = oi.Quantity * oi.UnitPrice
                }).ToList()
            })
            .FirstOrDefaultAsync(ct);

        if (order is null) return NotFound();

        return Ok(order);
    }
}
