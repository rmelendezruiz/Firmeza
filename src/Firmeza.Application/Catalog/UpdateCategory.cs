using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Catalog;

public sealed class UpdateCategory(ICategoryStore categories, IUnitOfWork work)
{
    public async Task<CategoryDto> ExecuteAsync(Guid id, SaveCategoryCommand command, CancellationToken ct)
    {
        var category = await categories.GetAsync(id, ct)
            ?? throw new NotFoundException("Categoría no encontrada.");
        category.Update(command.Name);
        if (await categories.NameExistsAsync(category.Name, category.Id, ct))
        {
            throw new BusinessRuleException("Ya existe una categoría con ese nombre.");
        }

        await work.SaveAsync(ct);
        return CategoryDto.From(category);
    }
}
