using System.Globalization;
using Firmeza.Application.Customers;
using Firmeza.Application.Errors;
using Firmeza.Application.Identity;
using Firmeza.Application.Ports;
using Firmeza.Application.Session;
using Firmeza.Application.Sales;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Storefront;

public sealed class SearchParts(
    IProductStore products,
    IBrandStore brands,
    ICategoryStore categories,
    IVehicleStore vehicles)
{
    public async Task<IReadOnlyList<CatalogPartDto>> ExecuteAsync(string? search, CancellationToken ct)
    {
        var items = await products.ListAsync(null, StockFilter.All, ct);
        var marcasPorId = (await brands.ListAsync(ct)).ToDictionary(brand => brand.Id, brand => brand.Name);
        var categoriasPorId = (await categories.ListAsync(ct))
            .ToDictionary(category => category.Id, category => category.Name);
        var vehiculosPorId = (await vehicles.ListAsync(ct)).ToDictionary(vehicle => vehicle.Id);

        var catalog = items.Select(product => new CatalogPartDto(
            product.Id,
            product.Sku,
            product.Name,
            product.Price,
            Buscar(marcasPorId, product.BrandId, "Marca no encontrada."),
            Buscar(categoriasPorId, product.CategoryId, "Categoría no encontrada."),
            product.Quantity > 0,
            product.Fitments
                .Select(fitment => Text(vehiculosPorId, fitment.VehicleId))
                .Where(text => text.Length > 0)
                .ToArray())).ToList();

        var text = search?.Trim();
        if (string.IsNullOrEmpty(text))
        {
            return catalog;
        }

        return catalog.Where(part => Coincide(part, text)).ToList();
    }

    private static bool Coincide(CatalogPartDto part, string text) =>
        part.Name.Contains(text, StringComparison.OrdinalIgnoreCase)
        || part.Sku.Contains(text, StringComparison.OrdinalIgnoreCase)
        || part.Brand.Contains(text, StringComparison.OrdinalIgnoreCase)
        || part.Category.Contains(text, StringComparison.OrdinalIgnoreCase)
        || part.Fitments.Any(fitment => fitment.Contains(text, StringComparison.OrdinalIgnoreCase));

    private static string Buscar(IReadOnlyDictionary<Guid, string> catalog, Guid id, string message)
    {
        if (!catalog.TryGetValue(id, out var name))
        {
            throw new NotFoundException(message);
        }

        return name;
    }

    private static string Text(IReadOnlyDictionary<Guid, Vehicle> vehicles, Guid id)
    {
        if (!vehicles.TryGetValue(id, out var vehicle))
        {
            return "";
        }

        return $"{vehicle.Brand} {vehicle.Line} {vehicle.Year}";
    }
}
