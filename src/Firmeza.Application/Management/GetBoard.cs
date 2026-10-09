using Firmeza.Application.Staff;
using Firmeza.Application.Errors;
using Firmeza.Application.Identity;
using Firmeza.Application.Ports;
using Firmeza.Application.Branches;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Management;

public sealed class GetBoard(
    StaffSession session,
    IStaffAccounts accounts,
    IManagementStore management,
    IBranchStore branches,
    ISaleStore sales,
    ICarStore cars)
{
    public async Task<BoardDto> ExecuteAsync(string? period, Guid? branchId, CancellationToken ct)
    {
        var yo = await session.RequireAsync(ct);
        if (yo.Role is not (Roles.Admin or Roles.CompanyManager or Roles.Manager))
        {
            throw new AccessDeniedException("Ese tablero es de la gerencia.");
        }

        var roster = await Roster.LoadAsync(accounts, management, branches, ct);
        var chosen = yo.SeesAllBranches
            ? branchId ?? roster.OrderedBranches.Select(branch => (Guid?)branch.Id).FirstOrDefault()
            : yo.OwnBranch();
        if (chosen is null || !roster.Branches.TryGetValue(chosen.Value, out var counter))
        {
            throw new NotFoundException("Counter no encontrado.");
        }

        var mes = Period.De(period);
        var activas = (await sales.ListBetweenAsync(mes.From, mes.Hasta, ct)).Where(sale => sale.IsActive).ToList();
        var targets = (await management.ListTargetsAsync(mes.Mes, ct))
            .ToDictionary(target => target.Email, StringComparer.OrdinalIgnoreCase);
        var summary = roster.Summarize(counter, activas, targets);
        var available = (await cars.ListAsync(counter.Id, ct)).Count(car => !car.Sold);
        return new BoardDto(
            mes.Mes,
            BranchSummaryDto.From(counter),
            roster.ManagerOf(counter.Id),
            summary.Sales,
            summary.Amount,
            summary.Target,
            summary.Progress,
            summary.OnSite,
            summary.AtCounter,
            summary.InInspection,
            available,
            summary.Rows,
            yo.SeesAllBranches ? roster.OrderedBranches.Select(BranchSummaryDto.From).ToList() : [],
            roster.Contacts(yo));
    }
}
