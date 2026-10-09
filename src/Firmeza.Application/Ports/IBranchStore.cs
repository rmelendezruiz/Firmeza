using Firmeza.Domain.Entities;

namespace Firmeza.Application.Ports;

public interface IBranchStore
{
    Task<IReadOnlyList<Branch>> ListAsync(CancellationToken ct);

    Task<Branch?> GetAsync(Guid id, CancellationToken ct);

    Task<Branch?> GetByCodeAsync(string code, CancellationToken ct);
}
