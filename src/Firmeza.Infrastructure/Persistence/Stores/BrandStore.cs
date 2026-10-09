using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Infrastructure.Persistence.Stores;

public sealed class BrandStore(FirmezaDbContext db) : IBrandStore
{
    public async Task<IReadOnlyList<Brand>> ListAsync(CancellationToken ct) =>
        await db.Brands.OrderBy(brand => brand.Name).ToListAsync(ct);

    public Task<Brand?> GetAsync(Guid id, CancellationToken ct) =>
        db.Brands.FirstOrDefaultAsync(brand => brand.Id == id, ct);

    public Task<bool> NameExistsAsync(string name, Guid? exceptoId, CancellationToken ct)
    {
        var normalizado = name.ToLower();
        return db.Brands.AnyAsync(
            brand => brand.Name.ToLower() == normalizado && (exceptoId == null || brand.Id != exceptoId),
            ct);
    }

    public Task<int> CountAsync(CancellationToken ct) => db.Brands.CountAsync(ct);

    public async Task AddAsync(Brand brand, CancellationToken ct) =>
        await db.Brands.AddAsync(brand, ct);

    public Task RemoveAsync(Brand brand, CancellationToken ct)
    {
        db.Brands.Remove(brand);
        return Task.CompletedTask;
    }
}
