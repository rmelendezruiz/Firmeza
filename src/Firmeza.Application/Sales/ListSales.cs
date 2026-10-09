using Firmeza.Application.Staff;
using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Sales;

public sealed class ListSales(
    StaffSession session,
    ISaleStore sales,
    ICustomerStore customers,
    IProductStore products,
    ICarStore cars,
    IBranchStore branches)
{
    public async Task<IReadOnlyList<SaleDto>> ExecuteAsync(CancellationToken ct)
    {
        var yo = await session.RequireAsync(ct);
        var items = await sales.ListAsync(yo.Scope, ct);
        var names = await SaleNames.LoadAsync(customers, products, cars, branches, ct);
        return items.Select(names.Dto).ToList();
    }
}
