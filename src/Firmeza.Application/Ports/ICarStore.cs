using Firmeza.Domain.Entities;

namespace Firmeza.Application.Ports;

public interface ICarStore
{
    Task<IReadOnlyList<Car>> ListAsync(Guid? branchId, CancellationToken ct);

    Task<IReadOnlyList<Car>> ListPublishedAsync(CancellationToken ct);

    Task<IReadOnlyList<Car>> ListAvailableAsync(ShowcaseFilter filter, CancellationToken ct);

    Task<Car?> GetAsync(Guid id, CancellationToken ct);

    Task<bool> HasSalesAsync(Guid id, CancellationToken ct);

    Task<int> CountPublishedAsync(CancellationToken ct);

    Task<int> CountAvailableAsync(CancellationToken ct);

    Task AddAsync(Car car, CancellationToken ct);

    Task RemoveAsync(Car car, CancellationToken ct);
}
