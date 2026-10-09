using Firmeza.Domain.Entities;

namespace Firmeza.Application.Ports;

public interface ICustomerStore
{
    Task<IReadOnlyList<Customer>> ListAsync(CancellationToken ct);

    Task<Customer?> GetAsync(Guid id, CancellationToken ct);

    Task<Customer?> GetByEmailAsync(string email, CancellationToken ct);

    Task<bool> EmailExistsAsync(string email, Guid? exceptoId, CancellationToken ct);

    Task<bool> DocumentExistsAsync(string document, Guid? exceptoId, CancellationToken ct);

    Task<int> CountAsync(CancellationToken ct);

    Task AddAsync(Customer customer, CancellationToken ct);

    Task RemoveAsync(Customer customer, CancellationToken ct);
}
