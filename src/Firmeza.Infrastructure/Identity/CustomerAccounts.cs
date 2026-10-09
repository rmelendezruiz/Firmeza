using Firmeza.Application.Identity;
using Firmeza.Application.Ports;
using Firmeza.Domain.Rules;
using Microsoft.AspNetCore.Identity;

namespace Firmeza.Infrastructure.Identity;

public sealed class CustomerAccounts(UserManager<IdentityUser> users) : ICustomerAccounts
{
    public async Task<bool> ExistsAsync(string email, CancellationToken ct) =>
        await users.FindByEmailAsync(email) is not null;

    public async Task CreateAsync(string email, string password, CancellationToken ct)
    {
        if (await users.FindByEmailAsync(email) is not null)
        {
            throw new BusinessRuleException("Ya existe una cuenta con ese email.");
        }

        var user = new IdentityUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
        };
        var created = await users.CreateAsync(user, password);
        if (!created.Succeeded)
        {
            throw new BusinessRuleException(Text(created));
        }

        var asignado = await users.AddToRoleAsync(user, Roles.Customer);
        if (!asignado.Succeeded)
        {
            throw new BusinessRuleException(Text(asignado));
        }
    }

    private static string Text(IdentityResult resultado) =>
        string.Join(" ", resultado.Errors.Select(error => error.Description));
}
