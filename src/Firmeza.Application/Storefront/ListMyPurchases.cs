using System.Globalization;
using Firmeza.Application.Customers;
using Firmeza.Application.Errors;
using Firmeza.Application.Identity;
using Firmeza.Application.Ports;
using Firmeza.Application.Session;
using Firmeza.Application.Sales;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Storefront;

public sealed class ListMyPurchases(
    IAdminIdentity identity,
    ICustomerStore customers,
    ICarStore cars,
    IBranchStore branches,
    ISaleStore sales)
{
    public async Task<IReadOnlyList<SitePurchaseDto>> ExecuteAsync(CancellationToken ct)
    {
        var card = await SessionCustomer.RequireAsync(identity, customers, ct);
        var items = await sales.ListByCustomerAsync(card.Id, ct);
        var names = (await cars.ListAsync(null, ct)).ToDictionary(car => car.Id, SaleNames.CarName);
        var counters = (await branches.ListAsync(ct)).ToDictionary(branch => branch.Id);
        return items
            .Select(sale => new SitePurchaseDto(
                sale.Number,
                sale.CarId is { } carId ? names.GetValueOrDefault(carId, "carro") : "repuestos",
                sale.Total,
                sale.InspectionDeposit,
                sale.InspectionDeadline,
                SaleText.Status(sale.Status),
                SaleText.Method(sale.PaymentMethod),
                counters.TryGetValue(sale.BranchId, out var branch) ? branch.Name : "counter",
                branch?.InspectionPhone ?? ""))
            .ToList();
    }
}
