using System.Text.RegularExpressions;
using Firmeza.Domain.Rules;

namespace Firmeza.Domain.Entities;

public sealed class SalesTarget
{
    private static readonly Regex PeriodoPatron = new(
        "^[0-9]{4}-(0[1-9]|1[0-2])$",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private SalesTarget()
    {
        Email = "";
        Period = "";
    }

    public Guid Id { get; private set; }

    public string Email { get; private set; }

    public string Period { get; private set; }

    public decimal Amount { get; private set; }

    public static SalesTarget Create(string? email, string? period, decimal amount)
    {
        return new SalesTarget
        {
            Id = Guid.NewGuid(),
            Email = Validation.Email(email),
            Period = FromPeriod(period),
            Amount = MontoDe(amount),
        };
    }

    public void Update(decimal amount)
    {
        Amount = MontoDe(amount);
    }

    public static string FromPeriod(string? period)
    {
        var clean = period?.Trim() ?? "";
        if (!PeriodoPatron.IsMatch(clean))
        {
            throw new BusinessRuleException("El período debe ser año-mes, por ejemplo 2026-10.");
        }

        return clean;
    }

    private static decimal MontoDe(decimal amount)
    {
        if (amount <= 0)
        {
            throw new BusinessRuleException("La meta tiene que ser mayor que cero.");
        }

        return decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
    }
}
