using Firmeza.Application.Staff;
using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Session;

internal static class Lockout
{
    public const string Mensaje = "La cuenta quedó bloqueada 15 minutos por intentos fallidos.";
}
