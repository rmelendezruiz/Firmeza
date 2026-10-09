using Firmeza.Domain.Entities;

namespace Firmeza.Application.Ports;

public interface IManagementStore
{
    Task<StaffCard?> GetStaffCardAsync(string email, CancellationToken ct);

    Task<IReadOnlyList<StaffCard>> ListStaffCardsAsync(CancellationToken ct);

    Task AddStaffCardAsync(StaffCard card, CancellationToken ct);

    Task<SalesTarget?> GetTargetAsync(string email, string period, CancellationToken ct);

    Task<IReadOnlyList<SalesTarget>> ListTargetsAsync(string period, CancellationToken ct);

    Task AddTargetAsync(SalesTarget target, CancellationToken ct);

    Task<IReadOnlyList<StaffMessage>> ListMessagesAsync(string email, string con, CancellationToken ct);

    Task AddMessageAsync(StaffMessage message, CancellationToken ct);
}
