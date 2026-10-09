using System.Globalization;
using Firmeza.Application.Customers;
using Firmeza.Application.Errors;
using Firmeza.Application.Identity;
using Firmeza.Application.Ports;
using Firmeza.Application.Session;
using Firmeza.Application.Sales;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Storefront;

public sealed class OpenAccount(
    ICustomerAccounts accounts,
    ICustomerStore customers,
    IUnitOfWork work)
{
    public async Task ExecuteAsync(OpenAccountCommand command, CancellationToken ct)
    {
        var proposed = Customer.Create(command.Name, command.Email, command.Document, command.Phone);
        var password = Validation.Password(command.Password);
        if (await accounts.ExistsAsync(proposed.Email, ct))
        {
            throw new BusinessRuleException("Ya existe una cuenta con ese email.");
        }

        var card = await customers.GetByEmailAsync(proposed.Email, ct);
        if (card is null)
        {
            await CreateCustomer.EnsureUniqueAsync(proposed, null, ct, customers);
            await accounts.CreateAsync(proposed.Email, password, ct);
            await customers.AddAsync(proposed, ct);
            await work.SaveAsync(ct);
            return;
        }

        if (!string.Equals(card.Document, proposed.Document, StringComparison.Ordinal))
        {
            throw new BusinessRuleException("Ese email ya tiene una ficha con otro documento.");
        }

        await accounts.CreateAsync(proposed.Email, password, ct);
    }
}
