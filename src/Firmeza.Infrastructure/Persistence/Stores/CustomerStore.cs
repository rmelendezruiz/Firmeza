using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Infrastructure.Persistence.Stores;

public sealed class CustomerStore(FirmezaDbContext db) : ICustomerStore
{
    public async Task<IReadOnlyList<Customer>> ListAsync(CancellationToken ct) =>
        await db.Customers.OrderBy(customer => customer.Name).ToListAsync(ct);

    public Task<Customer?> GetAsync(Guid id, CancellationToken ct) =>
        db.Customers.FirstOrDefaultAsync(customer => customer.Id == id, ct);

    public Task<Customer?> GetByEmailAsync(string email, CancellationToken ct) =>
        db.Customers.FirstOrDefaultAsync(customer => customer.Email == email, ct);

    public Task<bool> EmailExistsAsync(string email, Guid? exceptoId, CancellationToken ct) =>
        db.Customers.AnyAsync(
            customer => customer.Email == email && (exceptoId == null || customer.Id != exceptoId),
            ct);

    public Task<bool> DocumentExistsAsync(string document, Guid? exceptoId, CancellationToken ct) =>
        db.Customers.AnyAsync(
            customer => customer.Document == document && (exceptoId == null || customer.Id != exceptoId),
            ct);

    public Task<int> CountAsync(CancellationToken ct) => db.Customers.CountAsync(ct);

    public async Task AddAsync(Customer customer, CancellationToken ct) =>
        await db.Customers.AddAsync(customer, ct);

    public Task RemoveAsync(Customer customer, CancellationToken ct)
    {
        db.Customers.Remove(customer);
        return Task.CompletedTask;
    }
}
