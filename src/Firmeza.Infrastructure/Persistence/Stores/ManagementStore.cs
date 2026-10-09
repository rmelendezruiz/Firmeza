using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Infrastructure.Persistence.Stores;

public sealed class ManagementStore(FirmezaDbContext db) : IManagementStore
{
    public Task<StaffCard?> GetStaffCardAsync(string email, CancellationToken ct) =>
        db.StaffCards.FirstOrDefaultAsync(card => card.Email == email, ct);

    public async Task<IReadOnlyList<StaffCard>> ListStaffCardsAsync(CancellationToken ct) =>
        await db.StaffCards.OrderBy(card => card.Email).ToListAsync(ct);

    public async Task AddStaffCardAsync(StaffCard card, CancellationToken ct) =>
        await db.StaffCards.AddAsync(card, ct);

    public Task<SalesTarget?> GetTargetAsync(string email, string period, CancellationToken ct) =>
        db.Targets.FirstOrDefaultAsync(target => target.Email == email && target.Period == period, ct);

    public async Task<IReadOnlyList<SalesTarget>> ListTargetsAsync(string period, CancellationToken ct) =>
        await db.Targets.Where(target => target.Period == period).ToListAsync(ct);

    public async Task AddTargetAsync(SalesTarget target, CancellationToken ct) =>
        await db.Targets.AddAsync(target, ct);

    public async Task<IReadOnlyList<StaffMessage>> ListMessagesAsync(string email, string con, CancellationToken ct) =>
        await db.Messages
            .Where(message =>
                (message.FromEmail == email && message.ToEmail == con)
                || (message.FromEmail == con && message.ToEmail == email))
            .OrderBy(message => message.Date)
            .ToListAsync(ct);

    public async Task AddMessageAsync(StaffMessage message, CancellationToken ct) =>
        await db.Messages.AddAsync(message, ct);
}
