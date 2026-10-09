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

public sealed class ListShowcase(ICarStore cars, IBranchStore branches)
{
    public async Task<IReadOnlyList<PublishedCarDto>> ExecuteAsync(ShowcaseFilter? filter, CancellationToken ct)
    {
        var items = filter is null
            ? await cars.ListPublishedAsync(ct)
            : await cars.ListAvailableAsync(filter, ct);
        var counters = (await branches.ListAsync(ct)).ToDictionary(branch => branch.Id);
        return items
            .Where(car => counters.ContainsKey(car.BranchId))
            .Select(car => new PublishedCarDto(
                car.Id,
                car.Brand,
                car.Line,
                car.Year,
                car.Type == CarType.Truck ? "camion" : car.Type == CarType.Moto ? "moto" : "automovil",
                car.Condition == CarCondition.Used ? "usado" : "nuevo",
                car.Mileage,
                car.Availability == CarAvailability.SiteOnly ? "pagina"
                    : car.Availability == CarAvailability.CounterOnly ? "counter" : "ambos",
                car.PagePrice,
                car.CounterPrice,
                car.PhotoUrl,
                counters[car.BranchId].Code,
                counters[car.BranchId].Name,
                counters[car.BranchId].City))
            .ToList();
    }
}
