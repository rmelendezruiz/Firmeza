using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Products;

public sealed class GetProduct(
    IProductStore products,
    IBrandStore brands,
    ICategoryStore categories)
{
    public async Task<ProductDto> ExecuteAsync(Guid id, CancellationToken ct)
    {
        var product = await products.GetAsync(id, ct)
            ?? throw new NotFoundException("Producto no encontrado.");
        return await ProductMap.FromAsync(product, brands, categories, ct);
    }
}
