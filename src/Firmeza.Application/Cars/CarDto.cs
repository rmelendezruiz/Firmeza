using Firmeza.Domain.Entities;

namespace Firmeza.Application.Cars;

public sealed record CarDto(
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
    Guid BranchId,
    string Branch,
    bool Sold)
{
    public static CarDto From(Car car, string branch) =>
        new(
            car.Id,
            car.Brand,
            car.Line,
            car.Year,
            Name(car.Type),
            car.Condition == CarCondition.Used ? "usado" : "nuevo",
            car.Mileage,
            Name(car.Availability),
            car.PagePrice,
            car.CounterPrice,
            car.PhotoUrl,
            car.BranchId,
            branch,
            car.Sold);

    private static string Name(CarType type) => type switch
    {
        CarType.Truck => "camion",
        CarType.Moto => "moto",
        _ => "automovil",
    };

    private static string Name(CarAvailability availability) => availability switch
    {
        CarAvailability.SiteOnly => "pagina",
        CarAvailability.CounterOnly => "counter",
        _ => "ambos",
    };
}

public sealed record SaveCarCommand(
    string? Brand,
    string? Line,
    int Year,
    string? Type,
    string? Condition,
    int? Mileage,
    string? Availability,
    decimal? PagePrice,
    decimal? CounterPrice,
    string? PhotoUrl,
    Guid? BranchId);
