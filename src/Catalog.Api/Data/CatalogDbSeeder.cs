using Catalog.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Api.Data;

public static class CatalogDbSeeder
{
    public static async Task SeedAsync(CatalogDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Products.AnyAsync(cancellationToken))
        {
            return;
        }

        dbContext.Products.AddRange(
            new Product(Guid.Parse("11111111-1111-1111-1111-111111111111"), "Mechanical Keyboard", "Keyboard with hot-swappable switches", 129.99m),
            new Product(Guid.Parse("22222222-2222-2222-2222-222222222222"), "Gaming Mouse", "Lightweight mouse with optical sensor", 59.99m),
            new Product(Guid.Parse("33333333-3333-3333-3333-333333333333"), "USB-C Hub", "Multi-port adapter for laptops", 39.99m));

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}