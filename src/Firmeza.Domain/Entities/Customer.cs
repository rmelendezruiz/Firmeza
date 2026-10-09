using Firmeza.Domain.Rules;

namespace Firmeza.Domain.Entities;

public sealed class Customer
{
    private Customer()
    {
        Name = "";
        Email = "";
        Document = "";
        Phone = "";
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public string Email { get; private set; }

    public string Document { get; private set; }

    public string Phone { get; private set; }

    public static Customer Create(string? name, string? email, string? document, string? phone)
    {
        return new Customer
        {
            Id = Guid.NewGuid(),
            Name = Validation.Name(name),
            Email = Validation.Email(email),
            Document = Validation.Document(document),
            Phone = Validation.Phone(phone),
        };
    }

    public void Update(string? name, string? email, string? document, string? phone)
    {
        Name = Validation.Name(name);
        Email = Validation.Email(email);
        Document = Validation.Document(document);
        Phone = Validation.Phone(phone);
    }
}
