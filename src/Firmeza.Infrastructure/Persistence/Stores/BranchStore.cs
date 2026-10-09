using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Infrastructure.Persistence.Stores;

public sealed class BranchStore(FirmezaDbContext db) : IBranchStore
{
    public async Task<IReadOnlyList<Branch>> ListAsync(CancellationToken ct) =>
        await db.Branches
            .Include(branch => branch.Photos)
            .Include(branch => branch.Assets)
            .AsSplitQuery()
            .OrderBy(branch => branch.Code)
            .ToListAsync(ct);

    public Task<Branch?> GetAsync(Guid id, CancellationToken ct) =>
        db.Branches.FirstOrDefaultAsync(branch => branch.Id == id, ct);

    public Task<Branch?> GetByCodeAsync(string code, CancellationToken ct) =>
        db.Branches
            .Include(branch => branch.Photos)
            .Include(branch => branch.Assets)
            .AsSplitQuery()
            .FirstOrDefaultAsync(branch => branch.Code == code.Trim().ToUpper(), ct);
}
