using Firmeza.Domain.Rules;

namespace Firmeza.Domain.Entities;

public sealed class Category
{
    private Category()
    {
        Name = "";
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public static Category Create(string? name)
    {
        return new Category
        {
            Id = Guid.NewGuid(),
            Name = Validation.Name(name),
        };
    }

    public void Update(string? name)
    {
        Name = Validation.Name(name);
    }
}
