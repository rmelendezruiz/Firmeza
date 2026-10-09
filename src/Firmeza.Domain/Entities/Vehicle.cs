using Firmeza.Domain.Rules;

namespace Firmeza.Domain.Entities;

public sealed class Vehicle
{
    private Vehicle()
    {
        Brand = "";
        Line = "";
    }

    public Guid Id { get; private set; }

    public string Brand { get; private set; }

    public string Line { get; private set; }

    public int Year { get; private set; }

    public static Vehicle Create(string? brand, string? line, int year)
    {
        return new Vehicle
        {
            Id = Guid.NewGuid(),
            Brand = Validation.VehicleBrand(brand),
            Line = Validation.VehicleLine(line),
            Year = Validation.VehicleYear(year),
        };
    }

    public void Update(string? brand, string? line, int year)
    {
        Brand = Validation.VehicleBrand(brand);
        Line = Validation.VehicleLine(line);
        Year = Validation.VehicleYear(year);
    }
}
