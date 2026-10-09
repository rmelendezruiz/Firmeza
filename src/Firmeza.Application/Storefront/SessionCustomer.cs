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

internal static class SessionCustomer
{
    public static async Task<Customer> RequireAsync(IAdminIdentity identity, ICustomerStore customers, CancellationToken ct)
    {
        var session = await identity.CurrentAsync(ct)
            ?? throw new InvalidCredentialsException("No hay sesión.");
        if (session.Role != Roles.Customer)
        {
            throw new AccessDeniedException("La compra en línea es de la cuenta de cliente.");
        }

        return await customers.GetByEmailAsync(session.Email, ct)
            ?? throw new BusinessRuleException("Tu cuenta no tiene ficha de cliente.");
    }
}
