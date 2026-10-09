using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Customers;

public sealed class UpdateCustomer(ICustomerStore customers, IUnitOfWork work)
{
    public async Task<CustomerDto> ExecuteAsync(Guid id, SaveCustomerCommand command, CancellationToken ct)
    {
        var customer = await customers.GetAsync(id, ct)
            ?? throw new NotFoundException("Cliente no encontrado.");
        customer.Update(command.Name, command.Email, command.Document, command.Phone);
        await CreateCustomer.EnsureUniqueAsync(customer, customer.Id, ct, customers);
        await work.SaveAsync(ct);
        return CustomerDto.From(customer);
    }
}
