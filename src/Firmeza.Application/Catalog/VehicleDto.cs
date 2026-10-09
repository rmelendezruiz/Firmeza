using Firmeza.Domain.Entities;

namespace Firmeza.Application.Catalog;

public sealed record VehicleDto(Guid Id, string Brand, string Line, int Year)
{
    public static VehicleDto From(Vehicle vehicle) =>
        new(vehicle.Id, vehicle.Brand, vehicle.Line, vehicle.Year);
}

public sealed record SaveVehicleCommand(string? Brand, string? Line, int Year);
