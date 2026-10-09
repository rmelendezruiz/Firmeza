using Firmeza.Application.Staff;
using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Cars;

public sealed class DeleteCar(StaffSession session, ICarStore cars, IUnitOfWork work)
{
    public async Task ExecuteAsync(Guid id, CancellationToken ct)
    {
        var yo = await session.RequireAsync(ct);
        var car = await cars.GetAsync(id, ct);
        if (car is null || !yo.Reaches(car.BranchId))
        {
            throw new NotFoundException("Carro no encontrado.");
        }

        if (car.Sold || await cars.HasSalesAsync(id, ct))
        {
            throw new BusinessRuleException("El carro tiene una venta y no se puede eliminar.");
        }

        await cars.RemoveAsync(car, ct);
        await work.SaveAsync(ct);
    }
}
