using System.Text.RegularExpressions;

namespace Firmeza.Domain.Rules;

public static class Validation
{
    private static readonly Regex EmailPattern = new(
        "^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex DocumentPattern = new(
        "^[0-9]{6,12}$",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex PhonePattern = new(
        "^[0-9]{7,15}$",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);

    public static string Name(string? valor)
    {
        var clean = valor?.Trim() ?? "";
        if (clean.Length == 0)
        {
            throw new BusinessRuleException("El nombre es obligatorio.");
        }

        if (clean.Length > 120)
        {
            throw new BusinessRuleException("El nombre no puede pasar de 120 caracteres.");
        }

        return clean;
    }

    public static string Email(string? valor)
    {
        var clean = valor?.Trim().ToLowerInvariant() ?? "";
        if (clean.Length == 0)
        {
            throw new BusinessRuleException("El correo es obligatorio.");
        }

        if (clean.Length > 160 || !EmailPattern.IsMatch(clean))
        {
            throw new BusinessRuleException("El correo no es válido.");
        }

        return clean;
    }

    public static string Document(string? valor)
    {
        var clean = valor?.Trim() ?? "";
        if (clean.Length == 0)
        {
            throw new BusinessRuleException("El documento es obligatorio.");
        }

        if (!DocumentPattern.IsMatch(clean))
        {
            throw new BusinessRuleException("El documento debe tener entre 6 y 12 dígitos.");
        }

        return clean;
    }

    public static string Phone(string? valor)
    {
        var clean = valor?.Trim() ?? "";
        if (!PhonePattern.IsMatch(clean))
        {
            throw new BusinessRuleException("El teléfono debe tener entre 7 y 15 dígitos.");
        }

        return clean;
    }

    public static decimal Price(decimal valor)
    {
        if (valor <= 0)
        {
            throw new BusinessRuleException("El precio debe ser mayor que cero.");
        }

        if (valor != decimal.Round(valor, 2))
        {
            throw new BusinessRuleException("El precio admite como máximo dos decimales.");
        }

        if (valor > 99_999_999.99m)
        {
            throw new BusinessRuleException("El precio excede el máximo permitido.");
        }

        return valor;
    }

    public static int StockQuantity(int valor)
    {
        if (valor < 0)
        {
            throw new BusinessRuleException("La cantidad no puede ser negativa.");
        }

        if (valor > 1_000_000)
        {
            throw new BusinessRuleException("La cantidad excede el máximo permitido.");
        }

        return valor;
    }

    public static int SaleQuantity(int valor)
    {
        if (valor <= 0)
        {
            throw new BusinessRuleException("La cantidad de la venta debe ser mayor que cero.");
        }

        if (valor > 1_000_000)
        {
            throw new BusinessRuleException("La cantidad excede el máximo permitido.");
        }

        return valor;
    }

    public static string Password(string? valor)
    {
        var clean = valor ?? "";
        var hasSymbol = clean.Any(character => !char.IsLetterOrDigit(character));
        if (clean.Length < 8
            || !clean.Any(char.IsUpper)
            || !clean.Any(char.IsLower)
            || !clean.Any(char.IsDigit)
            || !hasSymbol)
        {
            throw new BusinessRuleException(
                "La clave necesita 8 caracteres, con mayúscula, minúscula, número y un signo.");
        }

        return clean;
    }

    public static string Sku(string? valor)
    {
        var clean = valor?.Trim().ToUpperInvariant() ?? "";
        if (clean.Length < 2 || clean.Length > 32 || !SkuPattern.IsMatch(clean))
        {
            throw new BusinessRuleException("El SKU debe tener de 2 a 32 caracteres: letras, números y guion.");
        }

        return clean;
    }

    public static Guid Required(Guid id, string message)
    {
        if (id == Guid.Empty)
        {
            throw new BusinessRuleException(message);
        }

        return id;
    }

    public static string VehicleBrand(string? valor) => Text(valor, "La marca del vehículo", 80);

    public static string VehicleLine(string? valor) => Text(valor, "La línea del vehículo", 80);

    public static int VehicleYear(int year)
    {
        var cap = DateTime.UtcNow.Year + 1;
        if (year < 1950 || year > cap)
        {
            throw new BusinessRuleException($"El año del vehículo debe estar entre 1950 y {cap}.");
        }

        return year;
    }

    private static string Text(string? valor, string field, int maxLength)
    {
        var clean = valor?.Trim() ?? "";
        if (clean.Length == 0)
        {
            throw new BusinessRuleException($"{field} es obligatoria.");
        }

        if (clean.Length > maxLength)
        {
            throw new BusinessRuleException($"{field} no puede pasar de {maxLength} caracteres.");
        }

        return clean;
    }

    private static readonly Regex SkuPattern = new(
        "^[A-Z0-9]+(?:-[A-Z0-9]+)*$",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);
}
