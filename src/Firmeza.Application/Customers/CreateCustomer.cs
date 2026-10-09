using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Customers;

public sealed class CreateCustomer(ICustomerStore customers, IUnitOfWork work)
{
    public async Task<CustomerDto> ExecuteAsync(SaveCustomerCommand command, CancellationToken ct)
    {
        var customer = Customer.Create(command.Name, command.Email, command.Document, command.Phone);
        await EnsureUniqueAsync(customer, null, ct);
        await customers.AddAsync(customer, ct);
        await work.SaveAsync(ct);
        return CustomerDto.From(customer);
    }

    internal static async Task EnsureUniqueAsync(
        Customer customer,
        Guid? exceptoId,
        CancellationToken ct,
        ICustomerStore customers)
    {
        if (await customers.DocumentExistsAsync(customer.Document, exceptoId, ct))
        {
            throw new BusinessRuleException("Ya existe un cliente con ese documento.");
        }

        if (await customers.EmailExistsAsync(customer.Email, exceptoId, ct))
        {
            throw new BusinessRuleException("Ya existe un cliente con ese email.");
        }
    }

    private Task EnsureUniqueAsync(Customer customer, Guid? exceptoId, CancellationToken ct) =>
        EnsureUniqueAsync(customer, exceptoId, ct, customers);
}
