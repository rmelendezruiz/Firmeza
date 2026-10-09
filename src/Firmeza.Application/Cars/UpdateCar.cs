using Firmeza.Application.Staff;
using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Cars;

public sealed class UpdateCar(StaffSession session, ICarStore cars, IBranchStore branches, IUnitOfWork work)
{
    public async Task<CarDto> ExecuteAsync(Guid id, SaveCarCommand command, CancellationToken ct)
    {
        var yo = await session.RequireAsync(ct);
        var car = await cars.GetAsync(id, ct);
        if (car is null || !yo.Reaches(car.BranchId))
        {
            throw new NotFoundException("Carro no encontrado.");
        }

        var branch = await CarBranch.ChooseAsync(yo, command.BranchId, branches, ct);
        car.Update(
            command.Brand,
            command.Line,
            command.Year,
            command.Type,
            command.Condition,
            command.Mileage,
            command.Availability,
            command.PagePrice,
            command.CounterPrice,
            command.PhotoUrl,
            branch.Id);
        await work.SaveAsync(ct);
        return CarDto.From(car, branch.Name);
    }
}
