using Microsoft.Extensions.Caching.Memory;

namespace Firmeza.Infrastructure.Identity;

public sealed class SessionCache(IMemoryCache cache)
{
    public void Save(string token, string userId, DateTimeOffset expires) =>
        cache.Set(Password(token), userId, expires);

    public bool Vigente(string token, out string userId)
    {
        if (cache.TryGetValue(Password(token), out string? id) && !string.IsNullOrEmpty(id))
        {
            userId = id;
            return true;
        }

        userId = "";
        return false;
    }

    public void Revocar(string token) => cache.Remove(Password(token));

    private static string Password(string token) => "sesion-admin:" + token;
}
