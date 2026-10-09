using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Catalog;

public sealed class DeleteCategory(
    ICategoryStore categories,
    IProductStore products,
    IUnitOfWork work)
{
    public async Task ExecuteAsync(Guid id, CancellationToken ct)
    {
        var category = await categories.GetAsync(id, ct)
            ?? throw new NotFoundException("Categoría no encontrada.");
        if (await products.HasCategoryAsync(id, ct))
        {
            throw new BusinessRuleException("La categoría tiene productos y no se puede eliminar.");
        }

        await categories.RemoveAsync(category, ct);
        await work.SaveAsync(ct);
    }
}
