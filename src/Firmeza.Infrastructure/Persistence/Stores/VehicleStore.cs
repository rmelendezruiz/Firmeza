using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Infrastructure.Persistence.Stores;

public sealed class VehicleStore(FirmezaDbContext db) : IVehicleStore
{
    public async Task<IReadOnlyList<Vehicle>> ListAsync(CancellationToken ct) =>
        await db.Vehicles
            .OrderBy(vehicle => vehicle.Brand)
            .ThenBy(vehicle => vehicle.Line)
            .ThenBy(vehicle => vehicle.Year)
            .ToListAsync(ct);

    public Task<Vehicle?> GetAsync(Guid id, CancellationToken ct) =>
        db.Vehicles.FirstOrDefaultAsync(vehicle => vehicle.Id == id, ct);

    public Task<bool> ExistsAsync(string brand, string line, int year, Guid? exceptoId, CancellationToken ct)
    {
        var marcaNormalizada = brand.ToLower();
        var lineaNormalizada = line.ToLower();
        return db.Vehicles.AnyAsync(
            vehicle => vehicle.Brand.ToLower() == marcaNormalizada
                && vehicle.Line.ToLower() == lineaNormalizada
                && vehicle.Year == year
                && (exceptoId == null || vehicle.Id != exceptoId),
            ct);
    }

    public Task<int> CountByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken ct) =>
        db.Vehicles.CountAsync(vehicle => ids.Contains(vehicle.Id), ct);

    public Task<bool> HasFitmentsAsync(Guid id, CancellationToken ct) =>
        db.Fitments.AnyAsync(fitment => fitment.VehicleId == id, ct);

    public Task<int> CountAsync(CancellationToken ct) => db.Vehicles.CountAsync(ct);

    public async Task AddAsync(Vehicle vehicle, CancellationToken ct) =>
        await db.Vehicles.AddAsync(vehicle, ct);

    public Task RemoveAsync(Vehicle vehicle, CancellationToken ct)
    {
        db.Vehicles.Remove(vehicle);
        return Task.CompletedTask;
    }
}
