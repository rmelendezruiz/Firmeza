using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Customers;

public sealed class GetCustomer(ICustomerStore customers)
{
    public async Task<CustomerDto> ExecuteAsync(Guid id, CancellationToken ct)
    {
        var customer = await customers.GetAsync(id, ct)
            ?? throw new NotFoundException("Cliente no encontrado.");
        return CustomerDto.From(customer);
    }
}
