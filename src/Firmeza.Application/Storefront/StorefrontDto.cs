namespace Firmeza.Application.Storefront;

public sealed record PublishedCarDto(
    Guid Id,
    string Brand,
    string Line,
    int Year,
    string Type,
    string Condition,
    int? Mileage,
    string Availability,
    decimal? PagePrice,
    decimal? CounterPrice,
    string PhotoUrl,
    string BranchCode,
    string Branch,
    string City);

public sealed record BuyOnSiteCommand(Guid CarId, bool AcceptsTerms, decimal SeenPrice, string? PaymentMethod);

public sealed record SitePurchaseDto(
    string Number,
    string Car,
    decimal Total,
    decimal? InspectionDeposit,
    DateTime? InspectionDeadline,
    string Status,
    string PaymentMethod,
    string Counter,
    string InspectionPhone);

public sealed record CatalogPartDto(
    Guid Id,
    string Sku,
    string Name,
    decimal Price,
    string Brand,
    string Category,
    bool Available,
    IReadOnlyList<string> Fitments);

public sealed record OpenAccountCommand(
    string? Name,
    string? Email,
    string? Document,
    string? Phone,
    string? Password);
