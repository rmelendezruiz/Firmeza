using Firmeza.Domain.Rules;

namespace Firmeza.Domain.Entities;

public sealed class Sale
{
    private readonly List<SaleLine> _lines = [];

    private Sale()
    {
        Number = "";
        SellerEmail = "";
    }

    public Guid Id { get; private set; }

    public string Number { get; private set; }

    public DateTime Date { get; private set; }

    public Guid CustomerId { get; private set; }

    public Guid BranchId { get; private set; }

    public SaleChannel Channel { get; private set; }

    public PaymentMethod PaymentMethod { get; private set; }

    public Guid? CarId { get; private set; }

    public decimal CarPrice { get; private set; }

    public SaleStatus Status { get; private set; }

    public decimal? InspectionDeposit { get; private set; }

    public DateTime? InspectionDeadline { get; private set; }

    public string SellerEmail { get; private set; }

    public decimal Iva => Tax.Calculate(TaxableBase).Iva;

    public decimal Total => Tax.Calculate(TaxableBase).Total;

    public bool IsActive => Status != SaleStatus.Cancelled;

    private decimal TaxableBase
    {
        get
        {
            var lines = 0m;
            foreach (var line in _lines)
            {
                lines += decimal.Round(line.Quantity * line.Price, 2, MidpointRounding.AwayFromZero);
            }

            return lines + (CarId is null ? 0 : CarPrice);
        }
    }

    public IReadOnlyCollection<SaleLine> Lines => _lines;

    public static Sale Register(
        string? number,
        DateTime date,
        Guid customerId,
        Guid branchId,
        SaleChannel channel,
        PaymentMethod paymentMethod,
        Guid? carId,
        decimal carPrice,
        IReadOnlyList<(Guid ProductId, int Quantity, decimal Price)> lines)
    {
        var cleanNumber = number?.Trim() ?? "";
        if (cleanNumber.Length == 0 || cleanNumber.Length > 20)
        {
            throw new BusinessRuleException("El número de venta es obligatorio.");
        }

        if (customerId == Guid.Empty)
        {
            throw new BusinessRuleException("La venta necesita un cliente.");
        }

        Validation.Required(branchId, "La venta necesita un counter.");
        if (channel == SaleChannel.Web && paymentMethod == PaymentMethod.Cash)
        {
            throw new BusinessRuleException("En la página se paga con transferencia, tarjeta o crédito.");
        }

        var hasCar = carId is not null && carId != Guid.Empty;
        if (hasCar && lines.Count > 0)
        {
            throw new BusinessRuleException("Registra el carro y los repuestos en ventas separadas.");
        }

        if (!hasCar && lines.Count == 0)
        {
            throw new BusinessRuleException("Elige un carro o un repuesto.");
        }

        if (hasCar)
        {
            carPrice = Validation.Price(carPrice);
        }

        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            Number = cleanNumber,
            Date = date,
            CustomerId = customerId,
            BranchId = branchId,
            Channel = channel,
            PaymentMethod = paymentMethod,
            CarId = hasCar ? carId : null,
            CarPrice = hasCar ? carPrice : 0m,
            Status = SaleStatus.Closed,
            SellerEmail = "",
        };

        var seen = new HashSet<Guid>();
        foreach (var line in lines)
        {
            if (!seen.Add(line.ProductId))
            {
                throw new BusinessRuleException("La venta repite un producto.");
            }

            var item = SaleLine.Create(sale.Id, line.ProductId, line.Quantity, line.Price);
            sale._lines.Add(item);
        }

        return sale;
    }

    public static PaymentMethod FromPaymentMethod(string? valor) => valor?.Trim().ToLowerInvariant() switch
    {
        "efectivo" => PaymentMethod.Cash,
        "transferencia" => PaymentMethod.Transfer,
        "tarjeta" => PaymentMethod.Card,
        "credito" or "crédito" => PaymentMethod.Credit,
        _ => throw new BusinessRuleException("El método de pago debe ser efectivo, transferencia, tarjeta o crédito."),
    };

    public void OpenInspection(decimal deposit, DateTime plazo)
    {
        if (Channel != SaleChannel.Web || CarId is null)
        {
            throw new BusinessRuleException("El peritaje solo aplica a un carro comprado en la página.");
        }

        if (deposit <= 0 || deposit >= Total)
        {
            throw new BusinessRuleException("El cargo preliminar tiene que ser menor que el total.");
        }

        InspectionDeposit = deposit;
        InspectionDeadline = plazo;
        Status = SaleStatus.Pending;
    }

    public void CloseInspection(DateTime ahora)
    {
        if (Status != SaleStatus.Pending)
        {
            throw new BusinessRuleException("Solo una compra en peritaje se puede cerrar.");
        }

        if (InspectionExpired(ahora))
        {
            throw new BusinessRuleException("El plazo del peritaje ya venció. Anula la compra.");
        }

        Status = SaleStatus.Closed;
    }

    public void Cancel()
    {
        if (Status != SaleStatus.Pending)
        {
            throw new BusinessRuleException("Solo una compra en peritaje se puede anular.");
        }

        Status = SaleStatus.Cancelled;
    }

    public bool InspectionExpired(DateTime ahora) =>
        Status == SaleStatus.Pending && InspectionDeadline is not null && InspectionDeadline < ahora;

    public void AssignSeller(string? email)
    {
        var clean = email?.Trim().ToLowerInvariant() ?? "";
        if (clean.Length > 160)
        {
            throw new BusinessRuleException("El correo del comercial no puede pasar de 160 caracteres.");
        }

        SellerEmail = clean;
    }
}
