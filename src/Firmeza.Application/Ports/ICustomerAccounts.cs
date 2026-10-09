namespace Firmeza.Application.Ports;

public interface ICustomerAccounts
{
    Task<bool> ExistsAsync(string email, CancellationToken ct);

    Task CreateAsync(string email, string password, CancellationToken ct);
}
