using Firmeza.Application.Errors;
using Firmeza.Application.Identity;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Staff;

internal static class BranchRules
{
    public static async Task<Branch?> ValidarAsync(
        string role,
        Guid? branchId,
        string email,
        IStaffAccounts accounts,
        IManagementStore management,
        IBranchStore branches,
        CancellationToken ct)
    {
        var chosen = branchId is null || branchId == Guid.Empty ? null : branchId;
        if (!Roles.HasBranch(role))
        {
            if (chosen is not null)
            {
                throw new BusinessRuleException(role == Roles.Admin
                    ? "La cuenta admin no lleva counter."
                    : "El gerente de Firmeza ve todos los counters y no lleva uno.");
            }

            return null;
        }

        if (chosen is null)
        {
            throw new BusinessRuleException("Elige el counter de la cuenta.");
        }

        var branch = await branches.GetAsync(chosen.Value, ct)
            ?? throw new NotFoundException("Counter no encontrado.");
        if (role != Roles.Manager)
        {
            return branch;
        }

        var gerentes = (await accounts.ListAsync(ct))
            .Where(account => account.Role == Roles.Manager && !account.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
            .Select(account => account.Email)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var ocupado = (await management.ListStaffCardsAsync(ct))
            .FirstOrDefault(card => card.BranchId == branch.Id && gerentes.Contains(card.Email));
        if (ocupado is not null)
        {
            throw new BusinessRuleException($"El counter {branch.Name} ya tiene gerente: {ocupado.Email}.");
        }

        return branch;
    }

    public static async Task AsignarAsync(string email, Branch? branch, IManagementStore management, CancellationToken ct)
    {
        var card = await management.GetStaffCardAsync(email, ct);
        if (card is null)
        {
            await management.AddStaffCardAsync(StaffCard.Create(email, branch?.Id), ct);
            return;
        }

        card.AssignBranch(branch?.Id);
    }
}
