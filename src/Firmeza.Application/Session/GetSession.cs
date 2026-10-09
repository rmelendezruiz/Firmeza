using Firmeza.Application.Staff;
using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Session;

public sealed class GetSession(StaffSession session, IBranchStore branches)
{
    public async Task<CurrentSessionDto> ExecuteAsync(CancellationToken ct)
    {
        var yo = await session.RequireAsync(ct);
        var branch = yo.BranchId is { } id ? await branches.GetAsync(id, ct) : null;
        return new CurrentSessionDto(yo.Email, yo.Role, branch?.Id, branch?.Name);
    }
}
