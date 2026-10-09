using Firmeza.Application.Ports;
using Firmeza.Domain.Rules;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Firmeza.Infrastructure.Persistence;

public sealed class UnitOfWork(FirmezaDbContext db) : IUnitOfWork
{
    public async Task SaveAsync(CancellationToken ct)
    {
        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (EsConflictoUnico(ex))
        {
            throw new BusinessRuleException("El registro entra en conflicto con uno existente.", ex);
        }
    }

    private static bool EsConflictoUnico(DbUpdateException ex) =>
        ex.InnerException is PostgresException postgres && postgres.SqlState == PostgresErrorCodes.UniqueViolation;
}
