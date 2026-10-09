namespace Firmeza.Domain.Rules;

public static class Inspection
{
    public const decimal DepositRate = 0.10m;

    public const int Days = 7;

    public static decimal Deposit(decimal pagePrice) =>
        decimal.Round(pagePrice * DepositRate, 2, MidpointRounding.AwayFromZero);
}
