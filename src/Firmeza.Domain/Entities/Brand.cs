using Firmeza.Domain.Rules;

namespace Firmeza.Domain.Entities;

public sealed class Brand
{
    private Brand()
    {
        Name = "";
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public static Brand Create(string? name)
    {
        return new Brand
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
