using Firmeza.Domain.Entities;

namespace Firmeza.Application.Ports;

public interface IVehicleStore
{
    Task<IReadOnlyList<Vehicle>> ListAsync(CancellationToken ct);

    Task<Vehicle?> GetAsync(Guid id, CancellationToken ct);

    Task<bool> ExistsAsync(string brand, string line, int year, Guid? exceptoId, CancellationToken ct);

    Task<int> CountByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken ct);

    Task<bool> HasFitmentsAsync(Guid id, CancellationToken ct);

    Task<int> CountAsync(CancellationToken ct);

    Task AddAsync(Vehicle vehicle, CancellationToken ct);

    Task RemoveAsync(Vehicle vehicle, CancellationToken ct);
}
