using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Infrastructure.Persistence.Stores;

public sealed class CarStore(FirmezaDbContext db) : ICarStore
{
    public async Task<IReadOnlyList<Car>> ListAsync(Guid? branchId, CancellationToken ct) =>
        await db.Cars
            .Where(car => branchId == null || car.BranchId == branchId)
            .OrderBy(car => car.Sold)
            .ThenBy(car => car.Brand)
            .ThenBy(car => car.Line)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Car>> ListPublishedAsync(CancellationToken ct) =>
        await db.Cars
            .Where(car => !car.Sold)
            .OrderBy(car => car.Brand)
            .ThenBy(car => car.Line)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Car>> ListAvailableAsync(ShowcaseFilter filter, CancellationToken ct)
    {
        var query = db.Cars.Where(car => !car.Sold);
        if (!string.IsNullOrWhiteSpace(filter.Brand))
        {
            var brand = filter.Brand.Trim().ToLower();
            query = query.Where(car => car.Brand.ToLower().Contains(brand));
        }

        if (!string.IsNullOrWhiteSpace(filter.Line))
        {
            var line = filter.Line.Trim().ToLower();
            query = query.Where(car => car.Line.ToLower().Contains(line));
        }

        if (!string.IsNullOrWhiteSpace(filter.City))
        {
            var city = filter.City.Trim().ToLower();
            query = query.Where(car =>
                db.Branches.Any(branch => branch.Id == car.BranchId && branch.City.ToLower() == city));
        }

        if (filter.YearFrom is not null)
        {
            query = query.Where(car => car.Year >= filter.YearFrom);
        }

        if (filter.YearTo is not null)
        {
            query = query.Where(car => car.Year <= filter.YearTo);
        }

        if (filter.PriceFrom is not null)
        {
            query = query.Where(car => (car.PagePrice ?? car.CounterPrice ?? 0) >= filter.PriceFrom);
        }

        if (filter.PriceTo is not null)
        {
            query = query.Where(car => (car.PagePrice ?? car.CounterPrice ?? 0) <= filter.PriceTo);
        }

        return await query
            .OrderBy(car => car.Brand)
            .ThenBy(car => car.Line)
            .ToListAsync(ct);
    }

    public Task<Car?> GetAsync(Guid id, CancellationToken ct) =>
        db.Cars.FirstOrDefaultAsync(car => car.Id == id, ct);

    public Task<bool> HasSalesAsync(Guid id, CancellationToken ct) =>
        db.Sales.AnyAsync(sale => sale.CarId == id, ct);

    public Task<int> CountPublishedAsync(CancellationToken ct) =>
        db.Cars.CountAsync(car => car.PagePrice != null && !car.Sold, ct);

    public Task<int> CountAvailableAsync(CancellationToken ct) =>
        db.Cars.CountAsync(car => !car.Sold, ct);

    public async Task AddAsync(Car car, CancellationToken ct) =>
        await db.Cars.AddAsync(car, ct);

    public Task RemoveAsync(Car car, CancellationToken ct)
    {
        db.Cars.Remove(car);
        return Task.CompletedTask;
    }
}
