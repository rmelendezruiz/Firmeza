using Firmeza.Application.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Firmeza.Infrastructure.Identity;

public sealed class IdentitySeed(
    RoleManager<IdentityRole> roles,
    UserManager<IdentityUser> users,
    IConfiguration configuration)
{
    public async Task ExecuteAsync(CancellationToken ct)
    {
        foreach (var role in Roles.All)
        {
            if (!await roles.RoleExistsAsync(role))
            {
                var created = await roles.CreateAsync(new IdentityRole(role));
                if (!created.Succeeded)
                {
                    throw new InvalidOperationException(Text(created));
                }
            }
        }

        foreach (var (previous, current) in Roles.Replacements)
        {
            if (!await roles.RoleExistsAsync(previous))
            {
                continue;
            }

            foreach (var user in await users.GetUsersInRoleAsync(previous))
            {
                if (!await users.IsInRoleAsync(user, current))
                {
                    Require(await users.AddToRoleAsync(user, current));
                }

                Require(await users.RemoveFromRoleAsync(user, previous));
            }
        }

        var email = configuration["InitialAdmin:Email"];
        var password = configuration["InitialAdmin:Password"];
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        var admin = await users.FindByEmailAsync(email);
        if (admin is null)
        {
            admin = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
            };
            Require(await users.CreateAsync(admin, password));
        }

        if (!await users.IsInRoleAsync(admin, Roles.Admin))
        {
            Require(await users.AddToRoleAsync(admin, Roles.Admin));
        }
    }

    private static void Require(IdentityResult resultado)
    {
        if (!resultado.Succeeded)
        {
            throw new InvalidOperationException(Text(resultado));
        }
    }

    private static string Text(IdentityResult resultado) =>
        string.Join("; ", resultado.Errors.Select(error => error.Description));
}
