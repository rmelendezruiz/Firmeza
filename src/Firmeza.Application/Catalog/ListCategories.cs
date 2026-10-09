using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Catalog;

public sealed class ListCategories(ICategoryStore categories)
{
    public async Task<IReadOnlyList<CategoryDto>> ExecuteAsync(CancellationToken ct)
    {
        var items = await categories.ListAsync(ct);
        return items.Select(CategoryDto.From).ToList();
    }
}
