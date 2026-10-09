using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Catalog;

public sealed class DeleteVehicle(IVehicleStore vehicles, IUnitOfWork work)
{
    public async Task ExecuteAsync(Guid id, CancellationToken ct)
    {
        var vehicle = await vehicles.GetAsync(id, ct)
            ?? throw new NotFoundException("Vehículo no encontrado.");
        if (await vehicles.HasFitmentsAsync(id, ct))
        {
            throw new BusinessRuleException("El vehículo tiene productos aplicados y no se puede eliminar.");
        }

        await vehicles.RemoveAsync(vehicle, ct);
        await work.SaveAsync(ct);
    }
}
