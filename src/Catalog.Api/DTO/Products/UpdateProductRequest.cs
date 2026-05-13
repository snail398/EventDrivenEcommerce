namespace Catalog.Api.DTO.Products;

public sealed record UpdateProductRequest(string Name, string Description, decimal Price);