using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure;

// This class defines how EF Core maps your C# entities to SQL tables.
// It knows about all DbSets (tables), relationships, and column configurations.
public class ECommerceDbContext : DbContext
{
    public ECommerceDbContext(DbContextOptions<ECommerceDbContext> options)
        : base(options) { }

    // === Tables (DbSets) ===
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();


    // === Model Configuration ===
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ------------------------------
        // Product
        // ------------------------------
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name)
                  .IsRequired()
                  .HasMaxLength(200);
            entity.Property(p => p.Category)
                  .HasMaxLength(100);
            entity.Property(p => p.Price)
                  .HasColumnType("decimal(18,2)");
            entity.Property(p => p.Stock)
                  .IsRequired();
        });

        // ------------------------------
        // Customer
        // ------------------------------
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name)
                  .IsRequired()
                  .HasMaxLength(150);
            entity.Property(c => c.Email)
                  .IsRequired()
                  .HasMaxLength(150);
        });

        // ------------------------------
        // Order
        // ------------------------------
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);

            // map property names to your SQL column names
            entity.Property(o => o.CreatedAt)
                  .HasColumnName("CreatedAtUtc");

            entity.Property(o => o.Total)
                  .HasColumnName("TotalAmount")
                  .HasColumnType("decimal(18,2)");

            // Enum will be stored as int automatically
            entity.Property(o => o.Status)
                  .HasConversion<int>();

            entity.HasOne(o => o.Customer)
                  .WithMany(c => c.Orders)
                  .HasForeignKey(o => o.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ------------------------------
        // OrderItem
        // ------------------------------
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.UnitPrice)
                  .HasColumnType("decimal(18,2)");

            entity.HasOne(i => i.Order)
                  .WithMany(o => o.Items)
                  .HasForeignKey(i => i.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(i => i.Product)
                  .WithMany(p => p.OrderItems)
                  .HasForeignKey(i => i.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
