using Catalog.Api.Models;

namespace Catalog.Api.Services;

public sealed class ProductService
{
    private readonly List<Product> _products =
    [
        new(Guid.NewGuid(), "Mechanical Keyboard", "Keyboard with hot-swappable switches", 129.99m),
        new(Guid.NewGuid(), "Gaming Mouse", "Lightweight mouse with optical sensor", 59.99m),
        new(Guid.NewGuid(), "USB-C Hub", "Multi-port adapter for laptops", 39.99m)
    ];

    public IEnumerable<Product> GetAll()
    {
        return _products;
    }

    public Product? GetById(Guid id)
    {
        return _products.FirstOrDefault(x => x.Id == id);
    }
}