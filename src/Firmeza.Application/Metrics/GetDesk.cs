using Firmeza.Application.Ports;
using Firmeza.Application.Sales;
using Firmeza.Domain.Entities;

namespace Firmeza.Application.Metrics;

public sealed class GetDesk(
    ISaleStore sales,
    ICarStore cars,
    ListSales listSales)
{
    public async Task<DeskDto> ExecuteAsync(CancellationToken ct)
    {
        var recientes = await listSales.ExecuteAsync(ct);
        return new DeskDto(
            await sales.CountByChannelAsync(SaleChannel.PhysicalStore, ct),
            await sales.CountByChannelAsync(SaleChannel.Web, ct),
            await cars.CountPublishedAsync(ct),
            await cars.CountAvailableAsync(ct),
            recientes.Take(5).ToList());
    }
}
