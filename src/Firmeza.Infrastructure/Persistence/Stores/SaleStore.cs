using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Infrastructure.Persistence.Stores;

public sealed class SaleStore(FirmezaDbContext db) : ISaleStore
{
    public async Task<IReadOnlyList<Sale>> ListAsync(Guid? branchId, CancellationToken ct) =>
        await db.Sales
            .Include(sale => sale.Lines)
            .Where(sale => branchId == null || sale.BranchId == branchId)
            .OrderByDescending(sale => sale.Date)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Sale>> ListByCustomerAsync(Guid customerId, CancellationToken ct) =>
        await db.Sales
            .Include(sale => sale.Lines)
            .Where(sale => sale.CustomerId == customerId)
            .OrderByDescending(sale => sale.Date)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Sale>> ListBetweenAsync(DateTime desde, DateTime hasta, CancellationToken ct) =>
        await db.Sales
            .Include(sale => sale.Lines)
            .Where(sale => sale.Date >= desde && sale.Date < hasta)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Sale>> ListExpiredInspectionsAsync(DateTime ahora, CancellationToken ct) =>
        await db.Sales
            .Include(sale => sale.Lines)
            .Where(sale => sale.Status == SaleStatus.Pending && sale.InspectionDeadline < ahora)
            .ToListAsync(ct);

    public Task<Sale?> GetAsync(Guid id, CancellationToken ct) =>
        db.Sales.Include(sale => sale.Lines).FirstOrDefaultAsync(sale => sale.Id == id, ct);

    public Task<string?> LastNumberAsync(CancellationToken ct) =>
        db.Sales.OrderByDescending(sale => sale.Number).Select(sale => sale.Number).FirstOrDefaultAsync(ct);

    public Task<bool> CustomerHasSalesAsync(Guid customerId, CancellationToken ct) =>
        db.Sales.AnyAsync(sale => sale.CustomerId == customerId, ct);

    public Task<bool> CustomerHasInspectionAsync(Guid customerId, CancellationToken ct) =>
        db.Sales.AnyAsync(sale => sale.CustomerId == customerId && sale.Status == SaleStatus.Pending, ct);

    public Task<bool> ProductHasSalesAsync(Guid productId, CancellationToken ct) =>
        db.SaleLines.AnyAsync(line => line.ProductId == productId, ct);

    public Task<int> CountByChannelAsync(SaleChannel channel, CancellationToken ct) =>
        db.Sales.CountAsync(sale => sale.Channel == channel && sale.Status != SaleStatus.Cancelled, ct);

    public async Task AddAsync(Sale sale, CancellationToken ct) =>
        await db.Sales.AddAsync(sale, ct);

    public Task RemoveAsync(Sale sale, CancellationToken ct)
    {
        db.Sales.Remove(sale);
        return Task.CompletedTask;
    }
}
