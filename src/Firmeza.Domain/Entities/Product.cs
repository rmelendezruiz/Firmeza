using Firmeza.Domain.Rules;

namespace Firmeza.Domain.Entities;

public sealed class Product
{
    private readonly List<Fitment> _fitments = [];

    private Product()
    {
        Name = "";
        Sku = "";
    }

    public Guid Id { get; private set; }

    public string Sku { get; private set; }

    public string Name { get; private set; }

    public decimal Price { get; private set; }

    public int Quantity { get; private set; }

    public Guid BrandId { get; private set; }

    public Guid CategoryId { get; private set; }

    public IReadOnlyCollection<Fitment> Fitments => _fitments;

    public static Product Create(
        string? name,
        decimal price,
        int quantity,
        string? sku,
        Guid brandId,
        Guid categoryId,
        IReadOnlyList<Guid>? vehicles)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = Validation.Name(name),
            Price = Validation.Price(price),
            Quantity = Validation.StockQuantity(quantity),
            Sku = Validation.Sku(sku),
            BrandId = Validation.Required(brandId, "La marca es obligatoria."),
            CategoryId = Validation.Required(categoryId, "La categoría es obligatoria."),
        };
        product.SetFitments(vehicles ?? []);
        return product;
    }

    public void Update(
        string? name,
        decimal price,
        int quantity,
        string? sku,
        Guid brandId,
        Guid categoryId,
        IReadOnlyList<Guid>? vehicles)
    {
        Name = Validation.Name(name);
        Price = Validation.Price(price);
        Quantity = Validation.StockQuantity(quantity);
        Sku = Validation.Sku(sku);
        BrandId = Validation.Required(brandId, "La marca es obligatoria.");
        CategoryId = Validation.Required(categoryId, "La categoría es obligatoria.");
        SetFitments(vehicles ?? []);
    }

    private void SetFitments(IReadOnlyList<Guid> vehicles)
    {
        var distintos = new HashSet<Guid>();
        var nuevas = new List<Fitment>();
        foreach (var vehicleId in vehicles)
        {
            if (!distintos.Add(vehicleId))
            {
                throw new BusinessRuleException("El producto repite un vehículo.");
            }

            nuevas.Add(Fitment.Create(Id, vehicleId));
        }

        _fitments.Clear();
        _fitments.AddRange(nuevas);
    }

    public void Deduct(int unidades)
    {
        var quantity = Validation.SaleQuantity(unidades);
        if (quantity > Quantity)
        {
            throw new BusinessRuleException($"No hay existencias suficientes de {Name}.");
        }

        Quantity -= quantity;
    }

    public void Restock(int unidades)
    {
        var quantity = Validation.SaleQuantity(unidades);
        Quantity = Validation.StockQuantity(Quantity + quantity);
    }
}
