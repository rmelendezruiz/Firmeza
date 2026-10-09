using Firmeza.Application.Staff;
using Firmeza.Application.Errors;
using Firmeza.Application.Identity;
using Firmeza.Application.Ports;
using Firmeza.Application.Branches;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Management;

public sealed class SaveTarget(
    StaffSession session,
    IStaffAccounts accounts,
    IManagementStore management,
    IBranchStore branches,
    IUnitOfWork work)
{
    public async Task ExecuteAsync(SaveTargetCommand command, CancellationToken ct)
    {
        var yo = await session.RequireAsync(ct);
        var email = Validation.Email(command.Email);
        var period = SalesTarget.FromPeriod(command.Period);
        var roster = await Roster.LoadAsync(accounts, management, branches, ct);
        var account = roster.Account(email) ?? throw new NotFoundException("Esa cuenta no está en el equipo.");
        roster.RequireCharge(yo, account, soloComercial: true);
        var target = await management.GetTargetAsync(email, period, ct);
        if (target is null)
        {
            await management.AddTargetAsync(SalesTarget.Create(email, period, command.Amount), ct);
        }
        else
        {
            target.Update(command.Amount);
        }

        await work.SaveAsync(ct);
    }
}
