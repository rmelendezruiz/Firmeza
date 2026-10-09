using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Catalog;

public sealed class CreateBrand(IBrandStore brands, IUnitOfWork work)
{
    public async Task<BrandDto> ExecuteAsync(SaveBrandCommand command, CancellationToken ct)
    {
        var brand = Brand.Create(command.Name);
        if (await brands.NameExistsAsync(brand.Name, null, ct))
        {
            throw new BusinessRuleException("Ya existe una marca con ese nombre.");
        }

        await brands.AddAsync(brand, ct);
        await work.SaveAsync(ct);
        return BrandDto.From(brand);
    }
}
