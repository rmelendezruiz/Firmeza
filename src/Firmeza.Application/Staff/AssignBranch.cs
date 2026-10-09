using Firmeza.Application.Errors;
using Firmeza.Application.Identity;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Staff;

public sealed class AssignBranch(
    IStaffAccounts accounts,
    IManagementStore management,
    IBranchStore branches,
    IUnitOfWork work)
{
    public async Task<AccountDto> ExecuteAsync(string id, AssignBranchCommand command, CancellationToken ct)
    {
        var account = (await accounts.ListAsync(ct)).FirstOrDefault(item => item.Id == id)
            ?? throw new NotFoundException("Cuenta no encontrada.");
        var branch = await BranchRules.ValidarAsync(account.Role, command.BranchId, account.Email, accounts, management, branches, ct);
        await BranchRules.AsignarAsync(account.Email, branch, management, ct);
        await work.SaveAsync(ct);
        return new AccountDto(account.Id, account.Email, account.Role, branch?.Id, branch?.Name);
    }
}
