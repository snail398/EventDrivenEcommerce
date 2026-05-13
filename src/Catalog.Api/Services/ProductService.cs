using Catalog.Api.DTO.Products;
using Catalog.Api.Messaging;
using Catalog.Api.Models;
using Catalog.Api.Repositories;
using Shared.Contracts.Events;

namespace Catalog.Api.Services;

public sealed class ProductService
{
    private readonly IProductRepository _productRepository;
    private readonly RabbitMqPublisher _publisher;

    public ProductService(IProductRepository productRepository, RabbitMqPublisher publisher)
    {
        _productRepository = productRepository;
        _publisher = publisher;
    }

    public Task<IReadOnlyCollection<Product>> GetAllAsync(CancellationToken cancellationToken)
    {
        return _productRepository.GetAllAsync(cancellationToken);
    }

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _productRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Product> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken)
    {
        var product = new Product(Guid.NewGuid(), request.Name, request.Description, request.Price);
        await _productRepository.AddAsync(product, cancellationToken);

        var @event = new ProductCreatedEvent(product.Id, product.Name, product.Price);
        await _publisher.PublishAsync("catalog.events", "product.created", @event, cancellationToken);

        return product;
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var product = new Product(id, request.Name, request.Description, request.Price);
        return await _productRepository.UpdateAsync(product, cancellationToken);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return _productRepository.DeleteAsync(id, cancellationToken);
    }
}