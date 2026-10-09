using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Infrastructure.Persistence.Stores;

public sealed class ProductStore(FirmezaDbContext db) : IProductStore
{
    public async Task<IReadOnlyList<Product>> ListAsync(
        string? search,
        StockFilter filter,
        CancellationToken ct)
    {
        var query = db.Products.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var text = Escapar(search.Trim());
            query = query.Where(product =>
                EF.Functions.ILike(product.Name, $"%{text}%", "\\")
                || EF.Functions.ILike(product.Sku, $"%{text}%", "\\"));
        }

        query = filter switch
        {
            StockFilter.InStock => query.Where(product => product.Quantity > 0),
            StockFilter.OutOfStock => query.Where(product => product.Quantity == 0),
            _ => query,
        };

        return await query.OrderBy(product => product.Name).ToListAsync(ct);
    }

    public Task<Product?> GetAsync(Guid id, CancellationToken ct) =>
        db.Products.FirstOrDefaultAsync(product => product.Id == id, ct);

    public Task<int> CountAsync(CancellationToken ct) => db.Products.CountAsync(ct);

    public Task<bool> SkuExistsAsync(string sku, Guid? exceptoId, CancellationToken ct) =>
        db.Products.AnyAsync(
            product => product.Sku == sku && (exceptoId == null || product.Id != exceptoId),
            ct);

    public Task<bool> HasBrandAsync(Guid brandId, CancellationToken ct) =>
        db.Products.AnyAsync(product => product.BrandId == brandId, ct);

    public Task<bool> HasCategoryAsync(Guid categoryId, CancellationToken ct) =>
        db.Products.AnyAsync(product => product.CategoryId == categoryId, ct);

    public async Task AddAsync(Product product, CancellationToken ct) =>
        await db.Products.AddAsync(product, ct);

    public Task RemoveAsync(Product product, CancellationToken ct)
    {
        db.Products.Remove(product);
        return Task.CompletedTask;
    }

    private static string Escapar(string text) =>
        text.Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("%", "\\%", StringComparison.Ordinal)
            .Replace("_", "\\_", StringComparison.Ordinal);
}
