using Firmeza.Application.Staff;
using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Cars;

public sealed class GetCar(StaffSession session, ICarStore cars, IBranchStore branches)
{
    public async Task<CarDto> ExecuteAsync(Guid id, CancellationToken ct)
    {
        var yo = await session.RequireAsync(ct);
        var car = await cars.GetAsync(id, ct);
        if (car is null || !yo.Reaches(car.BranchId))
        {
            throw new NotFoundException("Carro no encontrado.");
        }

        var branch = await branches.GetAsync(car.BranchId, ct);
        return CarDto.From(car, branch?.Name ?? "counter");
    }
}
