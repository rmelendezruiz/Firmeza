using Firmeza.Application.Staff;
using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Sales;

internal static class SaleNumber
{
    public static string Next(string? ultimo)
    {
        var current = 0;
        if (!string.IsNullOrWhiteSpace(ultimo)
            && ultimo.StartsWith("V-", StringComparison.Ordinal)
            && int.TryParse(ultimo.AsSpan(2), out var number))
        {
            current = number;
        }

        return $"V-{current + 1:000000}";
    }
}
