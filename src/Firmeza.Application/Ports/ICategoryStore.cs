using Firmeza.Domain.Entities;

namespace Firmeza.Application.Ports;

public interface ICategoryStore
{
    Task<IReadOnlyList<Category>> ListAsync(CancellationToken ct);

    Task<Category?> GetAsync(Guid id, CancellationToken ct);

    Task<bool> NameExistsAsync(string name, Guid? exceptoId, CancellationToken ct);

    Task<int> CountAsync(CancellationToken ct);

    Task AddAsync(Category category, CancellationToken ct);

    Task RemoveAsync(Category category, CancellationToken ct);
}
