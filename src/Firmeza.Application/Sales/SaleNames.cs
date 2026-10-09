using Firmeza.Application.Staff;
using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Sales;

internal sealed class SaleNames(
    IReadOnlyDictionary<Guid, string> customers,
    IReadOnlyDictionary<Guid, string> products,
    IReadOnlyDictionary<Guid, string> cars,
    IReadOnlyDictionary<Guid, string> branches)
{
    public static async Task<SaleNames> LoadAsync(
        ICustomerStore customers,
        IProductStore products,
        ICarStore cars,
        IBranchStore branches,
        CancellationToken ct)
    {
        return new SaleNames(
            (await customers.ListAsync(ct)).ToDictionary(customer => customer.Id, customer => customer.Name),
            (await products.ListAsync(null, StockFilter.All, ct)).ToDictionary(product => product.Id, product => product.Name),
            (await cars.ListAsync(null, ct)).ToDictionary(car => car.Id, CarName),
            (await branches.ListAsync(ct)).ToDictionary(branch => branch.Id, branch => branch.Name));
    }

    public SaleDto Dto(Sale sale) =>
        SaleDto.From(
            sale,
            customers.GetValueOrDefault(sale.CustomerId, "cliente"),
            branches.GetValueOrDefault(sale.BranchId, "counter"),
            products,
            sale.CarId is { } carId ? cars.GetValueOrDefault(carId) : null);

    public static string CarName(Car car) => $"{car.Brand} {car.Line} {car.Year}";
}
