namespace Firmeza.Domain.Rules;

public static class Tax
{
    public const decimal VatRate = 0.19m;

    public static (decimal Iva, decimal Total) Calculate(decimal taxableBase)
    {
        var iva = decimal.Round(taxableBase * VatRate, 2, MidpointRounding.AwayFromZero);
        var total = decimal.Round(taxableBase + iva, 2, MidpointRounding.AwayFromZero);
        return (iva, total);
    }
}
