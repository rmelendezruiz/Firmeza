using Firmeza.Application.Errors;
using Firmeza.Application.Identity;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Staff;

public sealed class ListAccounts(IStaffAccounts accounts, IManagementStore management, IBranchStore branches)
{
    public async Task<IReadOnlyList<AccountDto>> ExecuteAsync(CancellationToken ct)
    {
        var fichas = (await management.ListStaffCardsAsync(ct))
            .ToDictionary(card => card.Email, StringComparer.OrdinalIgnoreCase);
        var names = (await branches.ListAsync(ct)).ToDictionary(branch => branch.Id, branch => branch.Name);
        return (await accounts.ListAsync(ct))
            .Select(account =>
            {
                var branchId = fichas.TryGetValue(account.Email, out var card) ? card.BranchId : null;
                var name = branchId is { } id && names.TryGetValue(id, out var text) ? text : null;
                return new AccountDto(account.Id, account.Email, account.Role, branchId, name);
            })
            .ToList();
    }
}
