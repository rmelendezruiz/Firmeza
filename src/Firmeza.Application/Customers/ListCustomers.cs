using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Customers;

public sealed class ListCustomers(ICustomerStore customers)
{
    public async Task<IReadOnlyList<CustomerDto>> ExecuteAsync(CancellationToken ct)
    {
        var items = await customers.ListAsync(ct);
        return items.Select(CustomerDto.From).ToList();
    }
}
