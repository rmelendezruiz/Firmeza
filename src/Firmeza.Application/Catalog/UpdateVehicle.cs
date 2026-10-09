using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Catalog;

public sealed class UpdateVehicle(IVehicleStore vehicles, IUnitOfWork work)
{
    public async Task<VehicleDto> ExecuteAsync(Guid id, SaveVehicleCommand command, CancellationToken ct)
    {
        var vehicle = await vehicles.GetAsync(id, ct)
            ?? throw new NotFoundException("Vehículo no encontrado.");
        vehicle.Update(command.Brand, command.Line, command.Year);
        await CreateVehicle.EnsureUniqueAsync(vehicles, vehicle, vehicle.Id, ct);
        await work.SaveAsync(ct);
        return VehicleDto.From(vehicle);
    }
}
