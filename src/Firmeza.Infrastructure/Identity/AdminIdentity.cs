using System.Security.Claims;
using Firmeza.Application.Identity;
using Firmeza.Application.Ports;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Firmeza.Infrastructure.Identity;

public sealed class AdminIdentity(
    UserManager<IdentityUser> users,
    IDataProtectionProvider protection,
    SessionCache sesiones,
    IHttpContextAccessor http) : IAdminIdentity
{
    public async Task<SignInOutcome> SignInAsync(
        string email,
        string password,
        SessionGate puerta,
        CancellationToken ct)
    {
        var user = await users.FindByEmailAsync(email);
        if (user is null)
        {
            return SignInOutcome.Invalid();
        }

        if (await users.IsLockedOutAsync(user))
        {
            return SignInOutcome.Locked();
        }

        if (!await users.CheckPasswordAsync(user, password))
        {
            await users.AccessFailedAsync(user);
            return await users.IsLockedOutAsync(user) ? SignInOutcome.Locked() : SignInOutcome.Invalid();
        }

        await users.ResetAccessFailedCountAsync(user);
        var roles = await users.GetRolesAsync(user);
        var role = puerta == SessionGate.Customer
            ? RolCliente(roles)
            : Roles.FirstStaffRole(roles);
        if (role is null)
        {
            return SignInOutcome.Denied();
        }

        var expires = DateTimeOffset.UtcNow.AddHours(8);
        var token = SessionProtector.Protect(protection, user.Id, expires);
        sesiones.Save(token, user.Id, expires);
        return SignInOutcome.Succeeded(token, user.Email ?? email, role);
    }

    public Task<CurrentSession?> CurrentAsync(CancellationToken ct)
    {
        var user = http.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            return Task.FromResult<CurrentSession?>(null);
        }

        var email = user.FindFirstValue(ClaimTypes.Email) ?? "";
        var role = user.FindFirstValue(ClaimTypes.Role) ?? "";
        return Task.FromResult<CurrentSession?>(new CurrentSession(email, role));
    }

    public Task SignOutAsync(CancellationToken ct)
    {
        var encabezado = http.HttpContext?.Request.Headers.Authorization.ToString() ?? "";
        const string prefijo = "Bearer ";
        if (encabezado.StartsWith(prefijo, StringComparison.Ordinal))
        {
            sesiones.Revocar(encabezado[prefijo.Length..].Trim());
        }

        return Task.CompletedTask;
    }

    private static string? RolCliente(IList<string> roles) =>
        Roles.FirstStaffRole(roles) is null && roles.Contains(Roles.Customer) ? Roles.Customer : null;
}
