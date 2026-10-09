using Firmeza.Application.Staff;
using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Sales;

public sealed class GetSale(
    StaffSession session,
    ISaleStore sales,
    ICustomerStore customers,
    IProductStore products,
    ICarStore cars,
    IBranchStore branches)
{
    public async Task<SaleDto> ExecuteAsync(Guid id, CancellationToken ct)
    {
        var yo = await session.RequireAsync(ct);
        var sale = await sales.GetAsync(id, ct);
        if (sale is null || !yo.Reaches(sale.BranchId))
        {
            throw new NotFoundException("Venta no encontrada.");
        }

        var names = await SaleNames.LoadAsync(customers, products, cars, branches, ct);
        return names.Dto(sale);
    }
}
