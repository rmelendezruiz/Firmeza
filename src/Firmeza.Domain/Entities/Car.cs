using Firmeza.Domain.Rules;

namespace Firmeza.Domain.Entities;

public sealed class Car
{
    private Car()
    {
        Brand = "";
        Line = "";
        PhotoUrl = "";
    }

    public Guid Id { get; private set; }

    public string Brand { get; private set; }

    public string Line { get; private set; }

    public int Year { get; private set; }

    public CarType Type { get; private set; }

    public CarCondition Condition { get; private set; }

    public int? Mileage { get; private set; }

    public CarAvailability Availability { get; private set; }

    public decimal? PagePrice { get; private set; }

    public decimal? CounterPrice { get; private set; }

    public string PhotoUrl { get; private set; }

    public Guid BranchId { get; private set; }

    public bool Sold { get; private set; }

    public bool CanBuyOnSite => PagePrice is not null && !Sold;

    public static Car Create(
        string? brand,
        string? line,
        int year,
        string? type,
        string? condition,
        int? mileage,
        string? availability,
        decimal? pagePrice,
        decimal? counterPrice,
        string? photoUrl,
        Guid branchId)
    {
        var precios = Prices(availability, pagePrice, counterPrice);
        return new Car
        {
            Id = Guid.NewGuid(),
            Brand = Validation.VehicleBrand(brand),
            Line = Validation.VehicleLine(line),
            Year = Validation.VehicleYear(year),
            Type = FromType(type),
            Condition = FromCondition(condition),
            Mileage = MileageOf(FromCondition(condition), mileage),
            Availability = FromAvailability(availability),
            PagePrice = precios.Web,
            CounterPrice = precios.Counter,
            PhotoUrl = Photo(photoUrl),
            BranchId = Validation.Required(branchId, "El carro necesita un counter."),
        };
    }

    public void Update(
        string? brand,
        string? line,
        int year,
        string? type,
        string? condition,
        int? mileage,
        string? availability,
        decimal? pagePrice,
        decimal? counterPrice,
        string? photoUrl,
        Guid branchId)
    {
        if (Sold)
        {
            throw new BusinessRuleException("Un carro vendido no se edita.");
        }

        var cleanCondition = FromCondition(condition);
        var precios = Prices(availability, pagePrice, counterPrice);
        Brand = Validation.VehicleBrand(brand);
        Line = Validation.VehicleLine(line);
        Year = Validation.VehicleYear(year);
        Type = FromType(type);
        Condition = cleanCondition;
        Mileage = MileageOf(cleanCondition, mileage);
        Availability = FromAvailability(availability);
        PagePrice = precios.Web;
        CounterPrice = precios.Counter;
        PhotoUrl = Photo(photoUrl);
        BranchId = Validation.Required(branchId, "El carro necesita un counter.");
    }

    public decimal PriceOn(SaleChannel channel)
    {
        var price = channel == SaleChannel.Web ? PagePrice : CounterPrice;
        if (price is null)
        {
            throw new BusinessRuleException(
                channel == SaleChannel.Web
                    ? "Ese carro se compra en la sucursal."
                    : "Ese carro no tiene precio de counter.");
        }

        return price.Value;
    }

    public void Sell()
    {
        if (Sold)
        {
            throw new BusinessRuleException("Ese carro ya se vendió.");
        }

        Sold = true;
    }

    public void Release()
    {
        Sold = false;
    }

    public static CarType FromType(string? valor) => valor?.Trim().ToLowerInvariant() switch
    {
        "automovil" or "carro" => CarType.Automobile,
        "camion" => CarType.Truck,
        "moto" => CarType.Moto,
        _ => throw new BusinessRuleException("El tipo debe ser automóvil, camión o moto."),
    };

    public static CarCondition FromCondition(string? valor) => valor?.Trim().ToLowerInvariant() switch
    {
        "nuevo" => CarCondition.New,
        "usado" => CarCondition.Used,
        _ => throw new BusinessRuleException("La condición debe ser nuevo o usado."),
    };

    public static CarAvailability FromAvailability(string? valor) => valor?.Trim().ToLowerInvariant() switch
    {
        "pagina" or "solopagina" => CarAvailability.SiteOnly,
        "counter" or "solocounter" or "punto" => CarAvailability.CounterOnly,
        "ambos" => CarAvailability.Both,
        _ => throw new BusinessRuleException("La disponibilidad debe ser página, counter o ambos."),
    };

    private static int? MileageOf(CarCondition condition, int? mileage)
    {
        if (condition == CarCondition.New)
        {
            if (mileage is > 0)
            {
                throw new BusinessRuleException("Un carro nuevo no lleva kilometraje.");
            }

            return null;
        }

        if (mileage is null or <= 0)
        {
            throw new BusinessRuleException("El usado necesita el kilometraje.");
        }

        if (mileage > 2_000_000)
        {
            throw new BusinessRuleException("El kilometraje no puede pasar de 2000000.");
        }

        return mileage;
    }

    private static (decimal? Web, decimal? Counter) Prices(
        string? availability,
        decimal? pagePrice,
        decimal? counterPrice)
    {
        var donde = FromAvailability(availability);
        if (donde is CarAvailability.SiteOnly or CarAvailability.Both && pagePrice is null)
        {
            throw new BusinessRuleException("El precio de página es obligatorio.");
        }

        if (donde is CarAvailability.CounterOnly or CarAvailability.Both && counterPrice is null)
        {
            throw new BusinessRuleException("El precio de counter es obligatorio.");
        }

        decimal? web = donde is CarAvailability.SiteOnly or CarAvailability.Both
            ? Validation.Price(pagePrice!.Value)
            : null;
        decimal? counter = donde is CarAvailability.CounterOnly or CarAvailability.Both
            ? Validation.Price(counterPrice!.Value)
            : null;
        return (web, counter);
    }

    private static string Photo(string? valor)
    {
        var clean = valor?.Trim() ?? "";
        if (clean.Length > 300)
        {
            throw new BusinessRuleException("La foto no puede pasar de 300 caracteres.");
        }

        return clean;
    }
}
