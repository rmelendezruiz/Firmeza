using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Catalog;

public sealed class GetVehicle(IVehicleStore vehicles)
{
    public async Task<VehicleDto> ExecuteAsync(Guid id, CancellationToken ct)
    {
        var vehicle = await vehicles.GetAsync(id, ct)
            ?? throw new NotFoundException("Vehículo no encontrado.");
        return VehicleDto.From(vehicle);
    }
}
