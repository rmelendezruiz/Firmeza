using System.Globalization;
using Firmeza.Application.Customers;
using Firmeza.Application.Errors;
using Firmeza.Application.Identity;
using Firmeza.Application.Ports;
using Firmeza.Application.Session;
using Firmeza.Application.Sales;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Storefront;

public sealed class SignInCustomer(IAdminIdentity identity)
{
    public async Task<SessionDto> ExecuteAsync(SignInCommand command, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(command.Email) || string.IsNullOrWhiteSpace(command.Password))
        {
            throw new BusinessRuleException("Email y clave son obligatorios.");
        }

        var resultado = await identity.SignInAsync(command.Email.Trim(), command.Password, SessionGate.Customer, ct);
        return resultado.Status switch
        {
            SignInStatus.Succeeded => new SessionDto(resultado.Token!, resultado.Email!, resultado.Role!),
            SignInStatus.Denied => throw new AccessDeniedException("Esa cuenta entra por el equipo."),
            SignInStatus.Locked => throw new AccessDeniedException(Lockout.Mensaje),
            _ => throw new InvalidCredentialsException("Email o clave incorrectos."),
        };
    }
}
