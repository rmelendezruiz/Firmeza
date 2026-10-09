using Firmeza.Domain.Entities;

namespace Firmeza.Application.Ports;

public interface ISaleStore
{
    Task<IReadOnlyList<Sale>> ListAsync(Guid? branchId, CancellationToken ct);

    Task<IReadOnlyList<Sale>> ListByCustomerAsync(Guid customerId, CancellationToken ct);

    Task<IReadOnlyList<Sale>> ListBetweenAsync(DateTime desde, DateTime hasta, CancellationToken ct);

    Task<IReadOnlyList<Sale>> ListExpiredInspectionsAsync(DateTime ahora, CancellationToken ct);

    Task<Sale?> GetAsync(Guid id, CancellationToken ct);

    Task<string?> LastNumberAsync(CancellationToken ct);

    Task<bool> CustomerHasSalesAsync(Guid customerId, CancellationToken ct);

    Task<bool> CustomerHasInspectionAsync(Guid customerId, CancellationToken ct);

    Task<bool> ProductHasSalesAsync(Guid productId, CancellationToken ct);

    Task<int> CountByChannelAsync(SaleChannel channel, CancellationToken ct);

    Task AddAsync(Sale sale, CancellationToken ct);

    Task RemoveAsync(Sale sale, CancellationToken ct);
}
