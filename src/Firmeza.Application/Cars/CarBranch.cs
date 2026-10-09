using Firmeza.Application.Staff;
using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Cars;

internal static class CarBranch
{
    public static async Task<Branch> ChooseAsync(StaffSessionProfile yo, Guid? branchId, IBranchStore branches, CancellationToken ct)
    {
        var chosen = yo.SeesAllBranches ? branchId : branchId ?? yo.OwnBranch();
        if (chosen is null || chosen == Guid.Empty)
        {
            throw new BusinessRuleException("Elige el counter del carro.");
        }

        if (!yo.Reaches(chosen.Value))
        {
            throw new AccessDeniedException("Solo publicas carros en tu counter.");
        }

        return await branches.GetAsync(chosen.Value, ct)
            ?? throw new NotFoundException("Counter no encontrado.");
    }
}
