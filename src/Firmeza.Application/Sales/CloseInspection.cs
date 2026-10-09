using Firmeza.Application.Staff;
using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Sales;

public sealed class CloseInspection(StaffSession session, ISaleStore sales, IUnitOfWork work)
{
    public async Task ExecuteAsync(Guid id, CancellationToken ct)
    {
        var yo = await session.RequireAsync(ct);
        var sale = await sales.GetAsync(id, ct);
        if (sale is null || !yo.Reaches(sale.BranchId))
        {
            throw new NotFoundException("Venta no encontrada.");
        }

        sale.CloseInspection(DateTime.UtcNow);
        await work.SaveAsync(ct);
    }
}
