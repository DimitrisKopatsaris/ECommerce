using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ECommerce.Infrastructure;

public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ECommerceDbContext>
{
    public ECommerceDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ECommerceDbContext>()
            .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=ECommerceDb;Trusted_Connection=True;TrustServerCertificate=True;")
            .Options;

        return new ECommerceDbContext(options);
    }
}
