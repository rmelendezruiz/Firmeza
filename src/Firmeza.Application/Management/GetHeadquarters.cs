using Firmeza.Application.Staff;
using Firmeza.Application.Errors;
using Firmeza.Application.Identity;
using Firmeza.Application.Ports;
using Firmeza.Application.Branches;
using Firmeza.Application.Sales;
using Firmeza.Domain.Entities;

namespace Firmeza.Application.Management;

public sealed record AssetDto(string Name, string Type, int Quantity, string Status);

public sealed record HeadquartersCounterDto(
    BranchSummaryDto Branch,
    string Headquarters,
    string Phone,
    string Hours,
    int Capacity,
    int AreaM2,
    string? Manager,
    int Salespeople,
    int Sales,
    decimal Amount,
    decimal Target,
    decimal Progress,
    int OnSite,
    int AtCounter,
    int InInspection,
    PerformanceRowDto? TopSeller,
    int OnFloor,
    int InWarehouse,
    decimal InventoryValue,
    IReadOnlyList<AssetDto> Assets);

public sealed record PaymentMixDto(string Channel, string Method, int Sales, decimal Amount);

public sealed record TopCarDto(string City, string? Car, int Units, decimal Amount);

public sealed record LowStockPartDto(string Sku, string Name, int Quantity);

public sealed record InventoryDto(
    int Cars,
    int OnFloor,
    int InWarehouse,
    decimal Value,
    int Parts,
    int Units,
    IReadOnlyList<LowStockPartDto> LowStock);

public sealed record HeadquartersDto(
    string Period,
    int Sales,
    decimal Amount,
    int InInspection,
    IReadOnlyList<HeadquartersCounterDto> Counters,
    IReadOnlyList<PaymentMixDto> Payments,
    IReadOnlyList<TopCarDto> Top,
    IReadOnlyList<PerformanceRowDto> Ranking,
    InventoryDto Inventory,
    IReadOnlyList<ContactDto> Contacts);

public sealed class GetHeadquarters(
    StaffSession session,
    IStaffAccounts accounts,
    IManagementStore management,
    IBranchStore branches,
    ISaleStore sales,
    ICarStore cars,
    IProductStore products)
{
    public const int StockBajo = 3;

    public async Task<HeadquartersDto> ExecuteAsync(string? period, CancellationToken ct)
    {
        var yo = await session.RequireAsync(ct);
        if (!yo.SeesAllBranches)
        {
            throw new AccessDeniedException("La dirección es del gerente de Firmeza.");
        }

        var roster = await Roster.LoadAsync(accounts, management, branches, ct);
        var mes = Period.De(period);
        var activas = (await sales.ListBetweenAsync(mes.From, mes.Hasta, ct)).Where(sale => sale.IsActive).ToList();
        var targets = (await management.ListTargetsAsync(mes.Mes, ct))
            .ToDictionary(target => target.Email, StringComparer.OrdinalIgnoreCase);
        var flota = await cars.ListAsync(null, ct);
        var names = flota.ToDictionary(car => car.Id, car => $"{car.Brand} {car.Line}");
        var enStock = flota.Where(car => !car.Sold).ToList();

        var counters = roster.OrderedBranches
            .Select(branch => Counter(branch, roster, activas, targets, enStock))
            .ToList();
        var pagos = activas
            .GroupBy(sale => (sale.Channel, sale.PaymentMethod))
            .Select(grupo => new PaymentMixDto(
                SaleText.Channel(grupo.Key.Channel),
                SaleText.Method(grupo.Key.PaymentMethod),
                grupo.Count(),
                grupo.Sum(sale => sale.Total)))
            .OrderBy(pago => pago.Channel)
            .ThenByDescending(pago => pago.Sales)
            .ThenByDescending(pago => pago.Amount)
            .ToList();
        var top = roster.OrderedBranches
            .Select(branch => Top(branch, activas, names))
            .ToList();
        var correos = roster.Cuentas
            .Where(account => account.Role == Roles.Salesperson)
            .Select(account => account.Email)
            .Concat(activas.Where(sale => sale.SellerEmail.Length > 0).Select(sale => sale.SellerEmail));
        var parts = await products.ListAsync(null, StockFilter.All, ct);
        var inventario = new InventoryDto(
            enStock.Count,
            enStock.Count(car => car.Availability != CarAvailability.SiteOnly),
            enStock.Count(car => car.Availability == CarAvailability.SiteOnly),
            enStock.Sum(Valor),
            parts.Count,
            parts.Sum(product => product.Quantity),
            parts
                .Where(product => product.Quantity <= StockBajo)
                .OrderBy(product => product.Quantity)
                .ThenBy(product => product.Name)
                .Select(product => new LowStockPartDto(product.Sku, product.Name, product.Quantity))
                .ToList());

        return new HeadquartersDto(
            mes.Mes,
            activas.Count,
            activas.Sum(sale => sale.Total),
            activas.Count(sale => sale.Status == SaleStatus.Pending),
            counters,
            pagos,
            top,
            roster.Rows(correos, activas, targets),
            inventario,
            roster.Contacts(yo));
    }

    private static HeadquartersCounterDto Counter(
        Branch branch,
        Roster roster,
        IReadOnlyList<Sale> activas,
        IReadOnlyDictionary<string, SalesTarget> targets,
        IReadOnlyList<Car> enStock)
    {
        var summary = roster.Summarize(branch, activas, targets);
        var propios = enStock.Where(car => car.BranchId == branch.Id).ToList();
        return new HeadquartersCounterDto(
            BranchSummaryDto.From(branch),
            branch.Headquarters,
            branch.Phone,
            branch.Hours,
            branch.Capacity,
            branch.AreaM2,
            roster.ManagerOf(branch.Id),
            roster.ComercialesDe(branch.Id).Count(),
            summary.Sales,
            summary.Amount,
            summary.Target,
            summary.Progress,
            summary.OnSite,
            summary.AtCounter,
            summary.InInspection,
            summary.Rows.FirstOrDefault(row => row.Amount > 0),
            propios.Count(car => car.Availability != CarAvailability.SiteOnly),
            propios.Count(car => car.Availability == CarAvailability.SiteOnly),
            propios.Sum(Valor),
            branch.Assets
                .OrderBy(asset => asset.Type)
                .ThenBy(asset => asset.Name)
                .Select(asset => new AssetDto(
                    asset.Name,
                    asset.Type == AssetType.Equipment ? "equipo" : "infraestructura",
                    asset.Quantity,
                    asset.Status == AssetStatus.Maintenance ? "mantenimiento" : "operativo"))
                .ToList());
    }

    private static TopCarDto Top(Branch branch, IReadOnlyList<Sale> activas, IReadOnlyDictionary<Guid, string> names)
    {
        var mejor = activas
            .Where(sale => sale.BranchId == branch.Id && sale.CarId is not null)
            .GroupBy(sale => names.TryGetValue(sale.CarId!.Value, out var name) ? name : "carro")
            .Select(grupo => (Car: grupo.Key, Unidades: grupo.Count(), Amount: grupo.Sum(sale => sale.Total)))
            .OrderByDescending(item => item.Unidades)
            .ThenByDescending(item => item.Amount)
            .FirstOrDefault();
        return mejor.Car is null
            ? new TopCarDto(branch.City, null, 0, 0)
            : new TopCarDto(branch.City, mejor.Car, mejor.Unidades, mejor.Amount);
    }

    private static decimal Valor(Car car) => car.CounterPrice ?? car.PagePrice ?? 0;
}
