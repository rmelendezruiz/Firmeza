using Firmeza.Application.Sales;

namespace Firmeza.Staff.Jobs;

public sealed class InspectionExpiryJob(IServiceScopeFactory alcances, ILogger<InspectionExpiryJob> logger) : BackgroundService
{
    private static readonly TimeSpan Intervalo = TimeSpan.FromMinutes(10);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var reloj = new PeriodicTimer(Intervalo);
        do
        {
            try
            {
                await using var scope = alcances.CreateAsyncScope();
                var vencer = scope.ServiceProvider.GetRequiredService<ExpireInspections>();
                var anuladas = await vencer.ExecuteAsync(DateTime.UtcNow, stoppingToken);
                if (anuladas > 0)
                {
                    logger.LogInformation("Se anularon {Cantidad} compras con el peritaje vencido.", anuladas);
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "No se pudieron vencer los peritajes.");
            }
        }
        while (await reloj.WaitForNextTickAsync(stoppingToken));
    }
}
