using ECommerce.Domain.Entities;
using ECommerce.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Services;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ECommerceDbContext>();

        await db.Database.EnsureCreatedAsync();

        // Customers
        if (!await db.Customers.AnyAsync())
        {
            db.Customers.AddRange(
                new Customer { Name = "Alice Johnson", Email = "alice@example.com" },
                new Customer { Name = "Bob Smith",     Email = "bob@example.com" }
            );
        }

        // Products
        if (!await db.Products.AnyAsync())
        {
            db.Products.AddRange(
                new Product { Name = "Laptop 14\"",  Category = "Electronics", Price = 899.99m, Stock = 10 },
                new Product { Name = "Gaming Mouse", Category = "Electronics", Price = 49.90m,  Stock = 50 },
                new Product { Name = "Office Chair", Category = "Furniture",   Price = 129.00m, Stock = 15 }
            );
        }

        await db.SaveChangesAsync();

        // One sample order
        if (!await db.Orders.AnyAsync())
        {
            var customerId = await db.Customers.Select(c => c.Id).FirstAsync();
            var p = await db.Products.FirstAsync();

            var order = new Order
            {
                CustomerId = customerId,
                CreatedAt = DateTime.UtcNow,   // mapped to CreatedAtUtc
                Status = OrderStatus.Paid,
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductId = p.Id, Quantity = 2, UnitPrice = p.Price }
                }
            };

            order.Total = order.Items.Sum(i => i.Quantity * i.UnitPrice); // mapped to TotalAmount

            db.Orders.Add(order);
            await db.SaveChangesAsync();
        }
    }
}
