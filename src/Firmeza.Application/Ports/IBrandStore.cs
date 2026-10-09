using Firmeza.Domain.Entities;

namespace Firmeza.Application.Ports;

public interface IBrandStore
{
    Task<IReadOnlyList<Brand>> ListAsync(CancellationToken ct);

    Task<Brand?> GetAsync(Guid id, CancellationToken ct);

    Task<bool> NameExistsAsync(string name, Guid? exceptoId, CancellationToken ct);

    Task<int> CountAsync(CancellationToken ct);

    Task AddAsync(Brand brand, CancellationToken ct);

    Task RemoveAsync(Brand brand, CancellationToken ct);
}
