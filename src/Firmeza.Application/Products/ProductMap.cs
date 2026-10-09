using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Products;

internal static class ProductMap
{
    public static async Task<ProductDto> FromAsync(
        Product product,
        IBrandStore brands,
        ICategoryStore categories,
        CancellationToken ct)
    {
        var brand = await brands.GetAsync(product.BrandId, ct)
            ?? throw new NotFoundException("Marca no encontrada.");
        var category = await categories.GetAsync(product.CategoryId, ct)
            ?? throw new NotFoundException("Categoría no encontrada.");
        return ProductDto.From(product, brand.Name, category.Name);
    }
}
