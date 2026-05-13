namespace Catalog.Api.DTO.Products;

public sealed record CreateProductRequest(string Name, string Description, decimal Price);