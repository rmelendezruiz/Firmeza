using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Catalog;

public sealed class DeleteBrand(IBrandStore brands, IProductStore products, IUnitOfWork work)
{
    public async Task ExecuteAsync(Guid id, CancellationToken ct)
    {
        var brand = await brands.GetAsync(id, ct)
            ?? throw new NotFoundException("Marca no encontrada.");
        if (await products.HasBrandAsync(id, ct))
        {
            throw new BusinessRuleException("La marca tiene productos y no se puede eliminar.");
        }

        await brands.RemoveAsync(brand, ct);
        await work.SaveAsync(ct);
    }
}
