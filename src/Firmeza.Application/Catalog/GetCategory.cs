using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Catalog;

public sealed class GetCategory(ICategoryStore categories)
{
    public async Task<CategoryDto> ExecuteAsync(Guid id, CancellationToken ct)
    {
        var category = await categories.GetAsync(id, ct)
            ?? throw new NotFoundException("Categoría no encontrada.");
        return CategoryDto.From(category);
    }
}
