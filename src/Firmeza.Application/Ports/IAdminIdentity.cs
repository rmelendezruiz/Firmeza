namespace Firmeza.Application.Ports;

public enum SignInStatus
{
    Invalid,
    Denied,
    Locked,
    Succeeded,
}

public enum SessionGate
{
    Staff,
    Customer,
}

public sealed record SignInOutcome(SignInStatus Status, string? Token, string? Email, string? Role)
{
    public static SignInOutcome Invalid() => new(SignInStatus.Invalid, null, null, null);

    public static SignInOutcome Denied() => new(SignInStatus.Denied, null, null, null);

    public static SignInOutcome Locked() => new(SignInStatus.Locked, null, null, null);

    public static SignInOutcome Succeeded(string token, string email, string role) =>
        new(SignInStatus.Succeeded, token, email, role);
}

public sealed record CurrentSession(string Email, string Role);

public interface IAdminIdentity
{
    Task<SignInOutcome> SignInAsync(string email, string password, SessionGate puerta, CancellationToken ct);

    Task<CurrentSession?> CurrentAsync(CancellationToken ct);

    Task SignOutAsync(CancellationToken ct);
}
