using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Catalog;

public sealed class ListVehicles(IVehicleStore vehicles)
{
    public async Task<IReadOnlyList<VehicleDto>> ExecuteAsync(CancellationToken ct)
    {
        var items = await vehicles.ListAsync(ct);
        return items.Select(VehicleDto.From).ToList();
    }
}
