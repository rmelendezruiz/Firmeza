using Firmeza.Application.Staff;
using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Sales;

public sealed class CancelSale(
    StaffSession session,
    ISaleStore sales,
    ICarStore cars,
    IUnitOfWork work)
{
    public async Task ExecuteAsync(Guid id, CancellationToken ct)
    {
        var yo = await session.RequireAsync(ct);
        var sale = await sales.GetAsync(id, ct);
        if (sale is null || !yo.Reaches(sale.BranchId))
        {
            throw new NotFoundException("Venta no encontrada.");
        }

        sale.Cancel();
        await Release(sale, cars, ct);
        await work.SaveAsync(ct);
    }

    internal static async Task Release(Sale sale, ICarStore cars, CancellationToken ct)
    {
        if (sale.CarId is { } carId && await cars.GetAsync(carId, ct) is { } car)
        {
            car.Release();
        }
    }
}
