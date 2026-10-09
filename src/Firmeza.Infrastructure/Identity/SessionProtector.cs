using System.Security.Cryptography;
using Firmeza.Application.Identity;
using Microsoft.AspNetCore.DataProtection;

namespace Firmeza.Infrastructure.Identity;

public static class SessionProtector
{
    public const string Scheme = SessionScheme.Name;

    private const string Purpose = "Firmeza.StaffSession.v1";

    public static string Protect(IDataProtectionProvider protection, string userId, DateTimeOffset expires)
    {
        var protector = protection.CreateProtector(Purpose);
        return protector.Protect($"{userId}|{expires.ToUnixTimeSeconds()}");
    }

    public static bool TryRead(IDataProtectionProvider protection, string token, out string userId, out DateTimeOffset expires)
    {
        userId = "";
        expires = default;
        try
        {
            var protector = protection.CreateProtector(Purpose);
            var plain = protector.Unprotect(token);
            var separator = plain.IndexOf('|');
            if (separator <= 0 || separator == plain.Length - 1)
            {
                return false;
            }

            if (!long.TryParse(plain[(separator + 1)..], out var unix))
            {
                return false;
            }

            userId = plain[..separator];
            expires = DateTimeOffset.FromUnixTimeSeconds(unix);
            return userId.Length > 0 && expires > DateTimeOffset.UtcNow;
        }
        catch (CryptographicException)
        {
            return false;
        }
    }
}
