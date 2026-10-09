using System.Security.Claims;
using System.Text.Encodings.Web;
using Firmeza.Application.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Firmeza.Infrastructure.Identity;

public sealed class SessionAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var encabezado = Request.Headers.Authorization.ToString();
        const string prefijo = "Bearer ";
        if (!encabezado.StartsWith(prefijo, StringComparison.Ordinal))
        {
            return AuthenticateResult.NoResult();
        }

        var token = encabezado[prefijo.Length..].Trim();
        var protection = Context.RequestServices.GetRequiredService<IDataProtectionProvider>();
        var sesiones = Context.RequestServices.GetRequiredService<SessionCache>();
        if (!SessionProtector.TryRead(protection, token, out var userId, out _)
            || !sesiones.Vigente(token, out var userIdCache)
            || userIdCache != userId)
        {
            return AuthenticateResult.Fail("Sesión inválida.");
        }

        var users = Context.RequestServices.GetRequiredService<UserManager<IdentityUser>>();
        var user = await users.FindByIdAsync(userId);
        var roles = user is null ? [] : await users.GetRolesAsync(user);
        var role = Roles.SessionRole(roles);
        if (user is null || role is null)
        {
            return AuthenticateResult.Fail("Sesión inválida.");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim(ClaimTypes.Role, role),
        };
        var identity = new ClaimsIdentity(claims, SessionProtector.Scheme);
        return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), SessionProtector.Scheme));
    }
}
