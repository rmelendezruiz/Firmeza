using Firmeza.Domain.Entities;

namespace Firmeza.Application.Catalog;

public sealed record BrandDto(Guid Id, string Name)
{
    public static BrandDto From(Brand brand) => new(brand.Id, brand.Name);
}

public sealed record SaveBrandCommand(string? Name);
