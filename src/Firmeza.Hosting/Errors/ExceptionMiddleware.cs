using System.Runtime.ExceptionServices;
using Firmeza.Application.Errors;
using Firmeza.Domain.Rules;
using Firmeza.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;

namespace Firmeza.Hosting.Errors;

public sealed class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            await EscribirAsync(context, ex);
        }
    }

    private async Task EscribirAsync(HttpContext context, Exception ex)
    {
        var (status, message) = ex switch
        {
            BusinessRuleException regla => (StatusCodes.Status400BadRequest, regla.Message),
            NotFoundException noEncontrado => (StatusCodes.Status404NotFound, noEncontrado.Message),
            InvalidCredentialsException credenciales => (StatusCodes.Status401Unauthorized, credenciales.Message),
            AccessDeniedException acceso => (StatusCodes.Status403Forbidden, acceso.Message),
            ConflictException conflicto => (StatusCodes.Status409Conflict, conflicto.Message),
            _ when PersistenceConflict.TryMensaje(ex, out var choque) => (StatusCodes.Status409Conflict, choque),
            _ => (StatusCodes.Status500InternalServerError, "No se pudo completar la operación."),
        };

        if (status == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(ex, "Error no controlado");
        }

        if (context.Response.HasStarted)
        {
            ExceptionDispatchInfo.Capture(ex).Throw();
        }

        context.Response.Clear();
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { message });
    }
}
