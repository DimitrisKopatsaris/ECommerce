using ECommerce.Api.Dtos;                 // CreateProductRequest, UpdateProductRequest
using ECommerce.Api.Extensions;           // ApplyFiltering/ApplySorting
using ECommerce.Api.Models;               // ProductQuery
using ECommerce.Domain.Entities;          // Product
using ECommerce.Infrastructure;           // ECommerceDbContext
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProductsController : ControllerBase
{
    private readonly ECommerceDbContext _db;

    public ProductsController(ECommerceDbContext db) => _db = db;

    // GET /api/products?Q=&Category=&Sort=name|price&Dir=asc|desc&Page=1&PageSize=20
    [HttpGet]
    public async Task<ActionResult<List<Product>>> GetAll([FromQuery] ProductQuery query, CancellationToken ct)
    {
        var q = _db.Products.AsNoTracking();
        q = q.ApplyFiltering(query).ApplySorting(query);

        var page = query.Page <= 0 ? 1 : query.Page;
        var size = query.PageSize <= 0 ? 20 : Math.Min(query.PageSize, 100);

        var items = await q.Skip((page - 1) * size).Take(size).ToListAsync(ct);
        return Ok(items);
    }

    // GET /api/products/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetById(int id, CancellationToken ct)
    {
        var p = await _db.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return p is null ? NotFound() : Ok(p);
    }

    // POST /api/products
    [HttpPost]
    public async Task<ActionResult<Product>> Create([FromBody] CreateProductRequest req, CancellationToken ct)
    {
        var p = new Product
        {
            Name = req.Name,
            Price = req.Price,
            Stock = req.Stock,
            Category = req.Category
        };

        _db.Products.Add(p);
        await _db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(GetById), new { id = p.Id }, p);
    }

    // PUT /api/products/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequest req, CancellationToken ct)
    {
        var p = await _db.Products.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (p is null) return NotFound();

        p.Name = req.Name;
        p.Price = req.Price;
        p.Stock = req.Stock;
        p.Category = req.Category;

        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    // DELETE /api/products/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var p = await _db.Products.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (p is null) return NotFound();

        _db.Products.Remove(p);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}
