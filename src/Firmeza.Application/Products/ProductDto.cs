using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;

namespace Firmeza.Application.Products;

public sealed record ProductDto(
    Guid Id,
    string Sku,
    string Name,
    decimal Price,
    int Quantity,
    Guid BrandId,
    string BrandName,
    Guid CategoryId,
    string CategoryName,
    IReadOnlyList<Guid> VehicleIds)
{
    public static ProductDto From(Product product, string brandName, string categoryName) =>
        new(
            product.Id,
            product.Sku,
            product.Name,
            product.Price,
            product.Quantity,
            product.BrandId,
            brandName,
            product.CategoryId,
            categoryName,
            product.Fitments.Select(fitment => fitment.VehicleId).ToArray());
}

public sealed record SaveProductCommand(
    string? Name,
    decimal Price,
    int Quantity,
    string? Sku,
    Guid BrandId,
    Guid CategoryId,
    IReadOnlyList<Guid>? VehicleIds);

public sealed record ProductQuery(string? Search, StockFilter Filter);
