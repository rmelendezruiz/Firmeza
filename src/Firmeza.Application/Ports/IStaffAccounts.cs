namespace Firmeza.Application.Ports;

public sealed record StaffAccount(string Id, string Email, string Role);

public interface IStaffAccounts
{
    Task<IReadOnlyList<StaffAccount>> ListAsync(CancellationToken ct);

    Task<StaffAccount> CreateAsync(string email, string password, string role, CancellationToken ct);
}
