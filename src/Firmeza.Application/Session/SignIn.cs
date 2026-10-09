using Firmeza.Application.Staff;
using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Session;

public sealed class SignIn(IAdminIdentity identity)
{
    public async Task<SessionDto> ExecuteAsync(SignInCommand command, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(command.Email) || string.IsNullOrWhiteSpace(command.Password))
        {
            throw new BusinessRuleException("Email y clave son obligatorios.");
        }

        var resultado = await identity.SignInAsync(command.Email.Trim(), command.Password, SessionGate.Staff, ct);
        return resultado.Status switch
        {
            SignInStatus.Succeeded => new SessionDto(resultado.Token!, resultado.Email!, resultado.Role!),
            SignInStatus.Denied => throw new AccessDeniedException("Esa cuenta no entra al panel."),
            SignInStatus.Locked => throw new AccessDeniedException(Lockout.Mensaje),
            _ => throw new InvalidCredentialsException("Email o clave incorrectos."),
        };
    }
}
