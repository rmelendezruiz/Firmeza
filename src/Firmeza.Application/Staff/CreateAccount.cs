using Firmeza.Application.Errors;
using Firmeza.Application.Identity;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Staff;

public sealed class CreateAccount(
    IStaffAccounts accounts,
    IManagementStore management,
    IBranchStore branches,
    IUnitOfWork work)
{
    public async Task<AccountDto> ExecuteAsync(CreateAccountCommand command, CancellationToken ct)
    {
        var email = Validation.Email(command.Email);
        var password = Validation.Password(command.Password);
        var role = command.Role?.Trim() ?? "";
        if (!Roles.IsNewStaffRole(role))
        {
            throw new BusinessRuleException("El rol debe ser gerente de Firmeza, gerente o comercial.");
        }

        var branch = await BranchRules.ValidarAsync(role, command.BranchId, email, accounts, management, branches, ct);
        var account = await accounts.CreateAsync(email, password, role, ct);
        await BranchRules.AsignarAsync(email, branch, management, ct);
        await work.SaveAsync(ct);
        return new AccountDto(account.Id, account.Email, account.Role, branch?.Id, branch?.Name);
    }
}
