using Firmeza.Domain.Entities;

namespace Firmeza.Application.Ports;

public interface IProductStore
{
    Task<IReadOnlyList<Product>> ListAsync(string? search, StockFilter filter, CancellationToken ct);

    Task<Product?> GetAsync(Guid id, CancellationToken ct);

    Task<int> CountAsync(CancellationToken ct);

    Task<bool> SkuExistsAsync(string sku, Guid? exceptoId, CancellationToken ct);

    Task<bool> HasBrandAsync(Guid brandId, CancellationToken ct);

    Task<bool> HasCategoryAsync(Guid categoryId, CancellationToken ct);

    Task AddAsync(Product product, CancellationToken ct);

    Task RemoveAsync(Product product, CancellationToken ct);
}
