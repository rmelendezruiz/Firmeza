using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Catalog;

public sealed class ListBrands(IBrandStore brands)
{
    public async Task<IReadOnlyList<BrandDto>> ExecuteAsync(CancellationToken ct)
    {
        var items = await brands.ListAsync(ct);
        return items.Select(BrandDto.From).ToList();
    }
}
