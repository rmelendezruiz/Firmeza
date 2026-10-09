using Firmeza.Application.Staff;
using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Cars;

public sealed class CreateCar(StaffSession session, ICarStore cars, IBranchStore branches, IUnitOfWork work)
{
    public async Task<CarDto> ExecuteAsync(SaveCarCommand command, CancellationToken ct)
    {
        var yo = await session.RequireAsync(ct);
        var branch = await CarBranch.ChooseAsync(yo, command.BranchId, branches, ct);
        var car = Car.Create(
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
        await cars.AddAsync(car, ct);
        await work.SaveAsync(ct);
        return CarDto.From(car, branch.Name);
    }
}
