using Firmeza.Application.Staff;
using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Cars;

public sealed class ListCars(StaffSession session, ICarStore cars, IBranchStore branches)
{
    public async Task<IReadOnlyList<CarDto>> ExecuteAsync(CancellationToken ct)
    {
        var yo = await session.RequireAsync(ct);
        var names = (await branches.ListAsync(ct)).ToDictionary(branch => branch.Id, branch => branch.Name);
        var items = await cars.ListAsync(yo.Scope, ct);
        return items.Select(car => CarDto.From(car, names.GetValueOrDefault(car.BranchId, "counter"))).ToList();
    }
}
