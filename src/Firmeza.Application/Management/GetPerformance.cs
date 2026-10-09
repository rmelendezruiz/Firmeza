using Firmeza.Application.Staff;
using Firmeza.Application.Errors;
using Firmeza.Application.Identity;
using Firmeza.Application.Ports;
using Firmeza.Application.Branches;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Management;

public sealed class GetPerformance(
    StaffSession session,
    IStaffAccounts accounts,
    IManagementStore management,
    IBranchStore branches,
    ISaleStore sales)
{
    public async Task<PerformanceDto> ExecuteAsync(string? period, CancellationToken ct)
    {
        var yo = await session.RequireAsync(ct);
        if (yo.Role != Roles.Salesperson)
        {
            throw new AccessDeniedException("El desempeño es de la cuenta comercial.");
        }

        var roster = await Roster.LoadAsync(accounts, management, branches, ct);
        var mes = Period.De(period);
        var activas = (await sales.ListBetweenAsync(mes.From, mes.Hasta, ct)).Where(sale => sale.IsActive).ToList();
        var targets = (await management.ListTargetsAsync(mes.Mes, ct))
            .ToDictionary(target => target.Email, StringComparer.OrdinalIgnoreCase);
        var row = roster.Rows([yo.Email], activas, targets)[0];
        var counter = yo.BranchId is { } id && roster.Branches.TryGetValue(id, out var branch)
            ? BranchSummaryDto.From(branch)
            : null;
        return new PerformanceDto(mes.Mes, counter, row, roster.Contacts(yo));
    }
}
