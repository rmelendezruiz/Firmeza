namespace Firmeza.Application.Ports;

public sealed record ShowcaseFilter(
    string? Brand,
    string? Line,
    string? City,
    int? YearFrom,
    int? YearTo,
    decimal? PriceFrom,
    decimal? PriceTo);
