using Firmeza.Domain.Rules;

namespace Firmeza.Domain.Entities;

public sealed class Fitment
{
    private Fitment()
    {
    }

    public Guid Id { get; private set; }

    public Guid ProductId { get; private set; }

    public Guid VehicleId { get; private set; }

    public static Fitment Create(Guid productId, Guid vehicleId)
    {
        if (productId == Guid.Empty || vehicleId == Guid.Empty)
        {
            throw new BusinessRuleException("La aplicación del producto está incompleta.");
        }

        return new Fitment
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            VehicleId = vehicleId,
        };
    }
}
