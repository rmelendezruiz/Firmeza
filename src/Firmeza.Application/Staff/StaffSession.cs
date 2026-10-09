using Firmeza.Application.Errors;
using Firmeza.Application.Identity;
using Firmeza.Application.Ports;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Staff;

public sealed record StaffSessionProfile(string Email, string Role, Guid? BranchId)
{
    public bool SeesAllBranches => Roles.SeesAllBranches(Role);

    public bool Reaches(Guid branchId) => SeesAllBranches || BranchId == branchId;

    public Guid? Scope => SeesAllBranches ? null : BranchId ?? Guid.Empty;

    public Guid OwnBranch() =>
        BranchId ?? throw new BusinessRuleException("Tu cuenta no tiene counter asignado. Pídele al admin que te asigne uno.");
}

public sealed class StaffSession(IAdminIdentity identity, IManagementStore management)
{
    public async Task<StaffSessionProfile> RequireAsync(CancellationToken ct)
    {
        var session = await identity.CurrentAsync(ct)
            ?? throw new InvalidCredentialsException("No hay sesión.");
        if (Roles.FirstStaffRole([session.Role]) is null)
        {
            throw new AccessDeniedException("Esa cuenta no es del equipo.");
        }

        var card = await management.GetStaffCardAsync(session.Email, ct);
        return new StaffSessionProfile(session.Email, session.Role, card?.BranchId);
    }
}
