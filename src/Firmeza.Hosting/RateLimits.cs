using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace Firmeza.Hosting;

public static class RateLimits
{
    public const string SignIn = "ingreso";

    public const string Purchase = "purchase";

    public static IServiceCollection AddRateLimits(this IServiceCollection services)
    {
        return services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = async (contexto, ct) =>
            {
                contexto.HttpContext.Response.ContentType = "application/json";
                await contexto.HttpContext.Response.WriteAsJsonAsync(
                    new { message = "Demasiados intentos seguidos. Espera un minuto y vuelve a intentarlo." },
                    ct);
            };
            options.AddPolicy(SignIn, contexto => PorIp(contexto, 10));
            options.AddPolicy(Purchase, contexto => PorIp(contexto, 5));
        });
    }

    private static RateLimitPartition<string> PorIp(HttpContext contexto, int permitidas) =>
        RateLimitPartition.GetFixedWindowLimiter(
            contexto.Connection.RemoteIpAddress?.ToString() ?? "sin-ip",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = permitidas,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
            });
}
