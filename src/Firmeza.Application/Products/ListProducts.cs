using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Products;

public sealed class ListProducts(
    IProductStore products,
    IBrandStore brands,
    ICategoryStore categories)
{
    public async Task<IReadOnlyList<ProductDto>> ExecuteAsync(ProductQuery query, CancellationToken ct)
    {
        var items = await products.ListAsync(query.Search, query.Filter, ct);
        var marcasPorId = (await brands.ListAsync(ct)).ToDictionary(brand => brand.Id, brand => brand.Name);
        var categoriasPorId = (await categories.ListAsync(ct))
            .ToDictionary(category => category.Id, category => category.Name);
        return items.Select(product => ProductDto.From(
            product,
            Buscar(marcasPorId, product.BrandId, "Marca no encontrada."),
            Buscar(categoriasPorId, product.CategoryId, "Categoría no encontrada."))).ToList();
    }

    private static string Buscar(IReadOnlyDictionary<Guid, string> catalog, Guid id, string message)
    {
        if (!catalog.TryGetValue(id, out var name))
        {
            throw new NotFoundException(message);
        }

        return name;
    }
}
