using Firmeza.Application.Staff;
using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Sales;

public sealed class ExpireInspections(ISaleStore sales, ICarStore cars, IUnitOfWork work)
{
    public async Task<int> ExecuteAsync(DateTime ahora, CancellationToken ct)
    {
        var vencidas = await sales.ListExpiredInspectionsAsync(ahora, ct);
        foreach (var sale in vencidas)
        {
            sale.Cancel();
            await CancelSale.Release(sale, cars, ct);
        }

        if (vencidas.Count > 0)
        {
            await work.SaveAsync(ct);
        }

        return vencidas.Count;
    }
}
