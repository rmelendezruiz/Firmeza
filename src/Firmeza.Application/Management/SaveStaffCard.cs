using Firmeza.Application.Staff;
using Firmeza.Application.Errors;
using Firmeza.Application.Identity;
using Firmeza.Application.Ports;
using Firmeza.Application.Branches;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Management;

public sealed class SaveStaffCard(
    StaffSession session,
    IStaffAccounts accounts,
    IManagementStore management,
    IBranchStore branches,
    IUnitOfWork work)
{
    public async Task ExecuteAsync(SaveStaffCardCommand command, CancellationToken ct)
    {
        var yo = await session.RequireAsync(ct);
        var email = Validation.Email(command.Email);
        var roster = await Roster.LoadAsync(accounts, management, branches, ct);
        var account = roster.Account(email) ?? throw new NotFoundException("Esa cuenta no está en el equipo.");
        roster.RequireCharge(yo, account, soloComercial: false);
        var card = await management.GetStaffCardAsync(email, ct);
        if (card is null)
        {
            card = StaffCard.Create(email, null);
            await management.AddStaffCardAsync(card, ct);
        }

        card.Update(command.Salary, command.CommissionPercent);
        await work.SaveAsync(ct);
    }
}
