using Firmeza.Domain.Entities;

namespace Firmeza.Application.Customers;

public sealed record CustomerDto(Guid Id, string Name, string Email, string Document, string Phone)
{
    public static CustomerDto From(Customer customer) =>
        new(customer.Id, customer.Name, customer.Email, customer.Document, customer.Phone);
}

public sealed record SaveCustomerCommand(string? Name, string? Email, string? Document, string? Phone);
