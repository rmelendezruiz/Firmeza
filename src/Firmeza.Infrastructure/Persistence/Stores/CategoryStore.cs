using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Infrastructure.Persistence.Stores;

public sealed class CategoryStore(FirmezaDbContext db) : ICategoryStore
{
    public async Task<IReadOnlyList<Category>> ListAsync(CancellationToken ct) =>
        await db.Categories.OrderBy(category => category.Name).ToListAsync(ct);

    public Task<Category?> GetAsync(Guid id, CancellationToken ct) =>
        db.Categories.FirstOrDefaultAsync(category => category.Id == id, ct);

    public Task<bool> NameExistsAsync(string name, Guid? exceptoId, CancellationToken ct)
    {
        var normalizado = name.ToLower();
        return db.Categories.AnyAsync(
            category => category.Name.ToLower() == normalizado && (exceptoId == null || category.Id != exceptoId),
            ct);
    }

    public Task<int> CountAsync(CancellationToken ct) => db.Categories.CountAsync(ct);

    public async Task AddAsync(Category category, CancellationToken ct) =>
        await db.Categories.AddAsync(category, ct);

    public Task RemoveAsync(Category category, CancellationToken ct)
    {
        db.Categories.Remove(category);
        return Task.CompletedTask;
    }
}
