using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Catalog;

public sealed class GetBrand(IBrandStore brands)
{
    public async Task<BrandDto> ExecuteAsync(Guid id, CancellationToken ct)
    {
        var brand = await brands.GetAsync(id, ct)
            ?? throw new NotFoundException("Marca no encontrada.");
        return BrandDto.From(brand);
    }
}
