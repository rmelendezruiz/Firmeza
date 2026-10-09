using Firmeza.Application.Staff;
using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Session;

public sealed class SignOut(IAdminIdentity identity)
{
    public Task ExecuteAsync(CancellationToken ct) => identity.SignOutAsync(ct);
}
