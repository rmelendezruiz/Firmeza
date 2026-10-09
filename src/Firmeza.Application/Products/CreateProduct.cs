using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Products;

public sealed class CreateProduct(
    IProductStore products,
    IBrandStore brands,
    ICategoryStore categories,
    IVehicleStore vehicles,
    IUnitOfWork work)
{
    public async Task<ProductDto> ExecuteAsync(SaveProductCommand command, CancellationToken ct)
    {
        var product = Product.Create(
            command.Name,
            command.Price,
            command.Quantity,
            command.Sku,
            command.BrandId,
            command.CategoryId,
            command.VehicleIds);
        await EnsureSkuAsync(products, product.Sku, null, ct);
        await EnsureCatalogAsync(product, brands, categories, vehicles, ct);
        await products.AddAsync(product, ct);
        await work.SaveAsync(ct);
        return await ProductMap.FromAsync(product, brands, categories, ct);
    }

    internal static async Task EnsureSkuAsync(
        IProductStore products,
        string sku,
        Guid? exceptoId,
        CancellationToken ct)
    {
        if (await products.SkuExistsAsync(sku, exceptoId, ct))
        {
            throw new BusinessRuleException("Ya existe un producto con ese SKU.");
        }
    }

    internal static async Task EnsureCatalogAsync(
        Product product,
        IBrandStore brands,
        ICategoryStore categories,
        IVehicleStore vehicles,
        CancellationToken ct)
    {
        if (await brands.GetAsync(product.BrandId, ct) is null)
        {
            throw new BusinessRuleException("La marca no existe.");
        }

        if (await categories.GetAsync(product.CategoryId, ct) is null)
        {
            throw new BusinessRuleException("La categoría no existe.");
        }

        var ids = product.Fitments.Select(fitment => fitment.VehicleId).Distinct().ToArray();
        if (ids.Length == 0)
        {
            return;
        }

        var found = await vehicles.CountByIdsAsync(ids, ct);
        if (found != ids.Length)
        {
            throw new BusinessRuleException("Hay un vehículo que no existe.");
        }
    }
}
