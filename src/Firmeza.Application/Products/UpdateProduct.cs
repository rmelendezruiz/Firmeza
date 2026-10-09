using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Products;

public sealed class UpdateProduct(
    IProductStore products,
    IBrandStore brands,
    ICategoryStore categories,
    IVehicleStore vehicles,
    IUnitOfWork work)
{
    public async Task<ProductDto> ExecuteAsync(Guid id, SaveProductCommand command, CancellationToken ct)
    {
        var product = await products.GetAsync(id, ct)
            ?? throw new NotFoundException("Producto no encontrado.");
        product.Update(
            command.Name,
            command.Price,
            command.Quantity,
            command.Sku,
            command.BrandId,
            command.CategoryId,
            command.VehicleIds);
        await CreateProduct.EnsureSkuAsync(products, product.Sku, product.Id, ct);
        await CreateProduct.EnsureCatalogAsync(product, brands, categories, vehicles, ct);
        await work.SaveAsync(ct);
        return await ProductMap.FromAsync(product, brands, categories, ct);
    }
}
