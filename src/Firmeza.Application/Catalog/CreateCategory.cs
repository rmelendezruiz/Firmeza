using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Catalog;

public sealed class CreateCategory(ICategoryStore categories, IUnitOfWork work)
{
    public async Task<CategoryDto> ExecuteAsync(SaveCategoryCommand command, CancellationToken ct)
    {
        var category = Category.Create(command.Name);
        if (await categories.NameExistsAsync(category.Name, null, ct))
        {
            throw new BusinessRuleException("Ya existe una categoría con ese nombre.");
        }

        await categories.AddAsync(category, ct);
        await work.SaveAsync(ct);
        return CategoryDto.From(category);
    }
}
