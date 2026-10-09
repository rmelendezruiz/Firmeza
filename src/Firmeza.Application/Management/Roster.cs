using System.Globalization;
using Firmeza.Application.Staff;
using Firmeza.Application.Errors;
using Firmeza.Application.Identity;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Management;

internal sealed record Period(string Mes, DateTime From, DateTime Hasta)
{
    private static readonly TimeSpan Colombia = TimeSpan.FromHours(-5);

    public static Period De(string? valor)
    {
        var mes = string.IsNullOrWhiteSpace(valor)
            ? DateTime.UtcNow.Add(Colombia).ToString("yyyy-MM", CultureInfo.InvariantCulture)
            : SalesTarget.FromPeriod(valor);
        var inicio = DateTime.ParseExact(mes, "yyyy-MM", CultureInfo.InvariantCulture);
        var desde = DateTime.SpecifyKind(inicio - Colombia, DateTimeKind.Utc);
        return new Period(mes, desde, desde.AddMonths(1));
    }
}

internal sealed record BranchSummary(
    int Sales,
    decimal Amount,
    decimal Target,
    decimal Progress,
    int OnSite,
    int AtCounter,
    int InInspection,
    IReadOnlyList<PerformanceRowDto> Rows);

internal sealed class Roster
{
    private Roster(
        IReadOnlyList<StaffAccount> accounts,
        IReadOnlyDictionary<string, StaffCard> fichas,
        IReadOnlyDictionary<Guid, Branch> branches)
    {
        Cuentas = accounts;
        Fichas = fichas;
        Branches = branches;
    }

    public IReadOnlyList<StaffAccount> Cuentas { get; }

    public IReadOnlyDictionary<string, StaffCard> Fichas { get; }

    public IReadOnlyDictionary<Guid, Branch> Branches { get; }

    public IEnumerable<Branch> OrderedBranches => Branches.Values.OrderBy(branch => branch.Code, StringComparer.Ordinal);

    public static async Task<Roster> LoadAsync(
        IStaffAccounts accounts,
        IManagementStore management,
        IBranchStore branches,
        CancellationToken ct)
    {
        var items = await accounts.ListAsync(ct);
        var fichas = (await management.ListStaffCardsAsync(ct))
            .ToDictionary(card => card.Email, StringComparer.OrdinalIgnoreCase);
        var counters = (await branches.ListAsync(ct)).ToDictionary(branch => branch.Id);
        return new Roster(items, fichas, counters);
    }

    public Guid? BranchOf(string email) => Fichas.TryGetValue(email, out var card) ? card.BranchId : null;

    public string? NombreSede(Guid? branchId) =>
        branchId is { } id && Branches.TryGetValue(id, out var branch) ? branch.Name : null;

    public StaffAccount? Account(string email) =>
        Cuentas.FirstOrDefault(account => account.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

    public string? ManagerOf(Guid branchId) =>
        Cuentas.FirstOrDefault(account => account.Role == Roles.Manager && BranchOf(account.Email) == branchId)?.Email;

    public IEnumerable<string> ComercialesDe(Guid branchId) =>
        Cuentas.Where(account => account.Role == Roles.Salesperson && BranchOf(account.Email) == branchId).Select(account => account.Email);

    public bool CanTalk(StaffSessionProfile yo, StaffAccount otra)
    {
        if (otra.Email.Equals(yo.Email, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (yo.SeesAllBranches || Roles.SeesAllBranches(otra.Role))
        {
            return true;
        }

        var mismoCounter = yo.BranchId is not null && BranchOf(otra.Email) == yo.BranchId;
        return mismoCounter
            && ((yo.Role == Roles.Manager && otra.Role == Roles.Salesperson)
                || (yo.Role == Roles.Salesperson && otra.Role == Roles.Manager));
    }

    public IReadOnlyList<ContactDto> Contacts(StaffSessionProfile yo) =>
        Cuentas
            .Where(account => CanTalk(yo, account))
            .OrderBy(account => Array.IndexOf(Roles.All, account.Role))
            .ThenBy(account => account.Email, StringComparer.OrdinalIgnoreCase)
            .Select(account => new ContactDto(account.Email, account.Role, NombreSede(BranchOf(account.Email))))
            .ToList();

    public void RequireCharge(StaffSessionProfile yo, StaffAccount account, bool soloComercial)
    {
        if (soloComercial && account.Role != Roles.Salesperson)
        {
            throw new BusinessRuleException("La meta es del comercial.");
        }

        var permitido = yo.Role switch
        {
            Roles.Admin => account.Role is Roles.CompanyManager or Roles.Manager or Roles.Salesperson,
            Roles.CompanyManager => account.Role is Roles.Manager or Roles.Salesperson,
            Roles.Manager => account.Role == Roles.Salesperson && yo.BranchId is not null && BranchOf(account.Email) == yo.BranchId,
            _ => false,
        };
        if (!permitido)
        {
            throw new AccessDeniedException("Esa cuenta no está a tu cargo.");
        }
    }

    public IReadOnlyList<PerformanceRowDto> Rows(
        IEnumerable<string> correos,
        IReadOnlyList<Sale> activas,
        IReadOnlyDictionary<string, SalesTarget> targets)
    {
        var rows = new List<PerformanceRowDto>();
        foreach (var email in correos.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var propias = activas
                .Where(sale => sale.SellerEmail.Equals(email, StringComparison.OrdinalIgnoreCase))
                .ToList();
            var amount = propias.Sum(sale => sale.Total);
            var antesDeIva = propias.Sum(sale => sale.Total - sale.Iva);
            Fichas.TryGetValue(email, out var card);
            var target = targets.TryGetValue(email, out var registro) ? registro.Amount : 0;
            var porcentaje = card?.CommissionPercent ?? 0;
            rows.Add(new PerformanceRowDto(
                email,
                NombreSede(card?.BranchId),
                propias.Count,
                amount,
                target,
                Progress(amount, target),
                card?.Salary ?? 0,
                porcentaje,
                decimal.Round(antesDeIva * porcentaje / 100m, 2, MidpointRounding.AwayFromZero)));
        }

        return rows.OrderByDescending(row => row.Amount).ThenBy(row => row.Email).ToList();
    }

    public BranchSummary Summarize(Branch branch, IReadOnlyList<Sale> activas, IReadOnlyDictionary<string, SalesTarget> targets)
    {
        var propias = activas.Where(sale => sale.BranchId == branch.Id).ToList();
        var comerciales = ComercialesDe(branch.Id).ToList();
        var vendedores = propias
            .Where(sale => sale.SellerEmail.Length > 0)
            .Select(sale => sale.SellerEmail);
        var amount = propias.Sum(sale => sale.Total);
        var target = comerciales.Sum(email => targets.TryGetValue(email, out var registro) ? registro.Amount : 0);
        return new BranchSummary(
            propias.Count,
            amount,
            target,
            Progress(amount, target),
            propias.Count(sale => sale.Channel == SaleChannel.Web),
            propias.Count(sale => sale.Channel == SaleChannel.PhysicalStore),
            propias.Count(sale => sale.Status == SaleStatus.Pending),
            Rows(comerciales.Concat(vendedores), propias, targets));
    }

    private static decimal Progress(decimal amount, decimal target) =>
        target == 0 ? 0 : decimal.Round(amount / target * 100m, 1, MidpointRounding.AwayFromZero);
}
