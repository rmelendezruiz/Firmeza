using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Customers;

public sealed class DeleteCustomer(
    ICustomerStore customers,
    ISaleStore sales,
    IUnitOfWork work)
{
    public async Task ExecuteAsync(Guid id, CancellationToken ct)
    {
        var customer = await customers.GetAsync(id, ct)
            ?? throw new NotFoundException("Cliente no encontrado.");
        if (await sales.CustomerHasSalesAsync(id, ct))
        {
            throw new BusinessRuleException("El cliente tiene ventas y no se puede eliminar.");
        }

        await customers.RemoveAsync(customer, ct);
        await work.SaveAsync(ct);
    }
}
