namespace Catalog.Api.Models;

public sealed record Product(
    Guid Id,
    string Name,
    string Description,
    decimal Price);