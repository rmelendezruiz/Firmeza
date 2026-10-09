using Firmeza.Domain.Rules;

namespace Firmeza.Domain.Entities;

public sealed class SaleLine
{
    private SaleLine()
    {
    }

    public Guid Id { get; private set; }

    public Guid SaleId { get; private set; }

    public Guid ProductId { get; private set; }

    public int Quantity { get; private set; }

    public decimal Price { get; private set; }

    public static SaleLine Create(Guid saleId, Guid productId, int quantity, decimal price)
    {
        if (saleId == Guid.Empty || productId == Guid.Empty)
        {
            throw new BusinessRuleException("El detalle de venta está incompleto.");
        }

        return new SaleLine
        {
            Id = Guid.NewGuid(),
            SaleId = saleId,
            ProductId = productId,
            Quantity = Validation.SaleQuantity(quantity),
            Price = Validation.Price(price),
        };
    }
}
