using Firmeza.Domain.Entities;

namespace Firmeza.Application.Catalog;

public sealed record CategoryDto(Guid Id, string Name)
{
    public static CategoryDto From(Category category) => new(category.Id, category.Name);
}

public sealed record SaveCategoryCommand(string? Name);
