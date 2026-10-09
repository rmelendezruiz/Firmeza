using Firmeza.Application.Identity;
using Firmeza.Application.Ports;
using Firmeza.Domain.Rules;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Infrastructure.Identity;

public sealed class StaffAccounts(UserManager<IdentityUser> users) : IStaffAccounts
{
    public async Task<IReadOnlyList<StaffAccount>> ListAsync(CancellationToken ct)
    {
        var items = await users.Users.OrderBy(user => user.Email).ToListAsync(ct);
        var accounts = new List<StaffAccount>();
        foreach (var user in items)
        {
            var roles = await users.GetRolesAsync(user);
            var role = Roles.FirstStaffRole(roles);
            if (role is null || string.IsNullOrWhiteSpace(user.Email))
            {
                continue;
            }

            accounts.Add(new StaffAccount(user.Id, user.Email, role));
        }

        return accounts;
    }

    public async Task<StaffAccount> CreateAsync(string email, string password, string role, CancellationToken ct)
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

        var asignado = await users.AddToRoleAsync(user, role);
        if (!asignado.Succeeded)
        {
            throw new BusinessRuleException(Text(asignado));
        }

        return new StaffAccount(user.Id, email, role);
    }

    private static string Text(IdentityResult resultado) =>
        string.Join(" ", resultado.Errors.Select(error => error.Description));
}
