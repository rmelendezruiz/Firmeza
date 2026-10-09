using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Catalog;

public sealed class CreateVehicle(IVehicleStore vehicles, IUnitOfWork work)
{
    public async Task<VehicleDto> ExecuteAsync(SaveVehicleCommand command, CancellationToken ct)
    {
        var vehicle = Vehicle.Create(command.Brand, command.Line, command.Year);
        await EnsureUniqueAsync(vehicles, vehicle, null, ct);
        await vehicles.AddAsync(vehicle, ct);
        await work.SaveAsync(ct);
        return VehicleDto.From(vehicle);
    }

    internal static async Task EnsureUniqueAsync(
        IVehicleStore vehicles,
        Vehicle vehicle,
        Guid? exceptoId,
        CancellationToken ct)
    {
        if (await vehicles.ExistsAsync(vehicle.Brand, vehicle.Line, vehicle.Year, exceptoId, ct))
        {
            throw new BusinessRuleException("Ya existe ese vehículo.");
        }
    }
}
