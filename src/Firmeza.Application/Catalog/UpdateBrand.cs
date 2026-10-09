using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Catalog;

public sealed class UpdateBrand(IBrandStore brands, IUnitOfWork work)
{
    public async Task<BrandDto> ExecuteAsync(Guid id, SaveBrandCommand command, CancellationToken ct)
    {
        var brand = await brands.GetAsync(id, ct)
            ?? throw new NotFoundException("Marca no encontrada.");
        brand.Update(command.Name);
        if (await brands.NameExistsAsync(brand.Name, brand.Id, ct))
        {
            throw new BusinessRuleException("Ya existe una marca con ese nombre.");
        }

        await work.SaveAsync(ct);
        return BrandDto.From(brand);
    }
}
