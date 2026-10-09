using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Firmeza.Infrastructure.Persistence;

public static class PersistenceConflict
{
    public static bool TryMensaje(Exception ex, out string message)
    {
        if (ex is DbUpdateException { InnerException: PostgresException { SqlState: PostgresErrorCodes.UniqueViolation } choque })
        {
            message = choque.ConstraintName switch
            {
                "IX_ventas_carro_id" => "Ese carro ya se vendió.",
                "IX_ventas_numero" => "Entró otra venta al mismo tiempo. Vuelve a intentarlo.",
                _ => "Ese registro ya existe.",
            };
            return true;
        }

        message = "";
        return false;
    }
}
