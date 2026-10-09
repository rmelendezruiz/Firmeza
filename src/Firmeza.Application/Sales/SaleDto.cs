using Firmeza.Domain.Entities;

namespace Firmeza.Application.Sales;

public sealed record SaleLineDto(Guid ProductId, string ProductName, int Quantity, decimal Price);

public sealed record SaleDto(
    Guid Id,
    string Number,
    DateTime Date,
    Guid CustomerId,
    string CustomerName,
    Guid BranchId,
    string Branch,
    string Channel,
    string PaymentMethod,
    string Status,
    Guid? CarId,
    string? CarName,
    decimal Total,
    decimal Iva,
    decimal? InspectionDeposit,
    DateTime? InspectionDeadline,
    string SellerEmail,
    IReadOnlyList<SaleLineDto> Lines)
{
    public static SaleDto From(
        Sale sale,
        string customerName,
        string branch,
        IReadOnlyDictionary<Guid, string> productNames,
        string? carName)
    {
        var lines = sale.Lines
            .Select(line => new SaleLineDto(
                line.ProductId,
                productNames.TryGetValue(line.ProductId, out var name) ? name : "producto",
                line.Quantity,
                line.Price))
            .ToList();

        return new SaleDto(
            sale.Id,
            sale.Number,
            sale.Date,
            sale.CustomerId,
            customerName,
            sale.BranchId,
            branch,
            SaleText.Channel(sale.Channel),
            SaleText.Method(sale.PaymentMethod),
            SaleText.Status(sale.Status),
            sale.CarId,
            carName,
            sale.Total,
            sale.Iva,
            sale.InspectionDeposit,
            sale.InspectionDeadline,
            sale.SellerEmail,
            lines);
    }
}

public sealed record SaleLineCommand(Guid ProductId, int Quantity);

public sealed record RegisterSaleCommand(
    Guid CustomerId,
    string? Channel,
    string? PaymentMethod,
    Guid? CarId,
    IReadOnlyList<SaleLineCommand>? Lines);

internal static class SaleText
{
    public static string Channel(SaleChannel channel) => channel == SaleChannel.Web ? "pagina" : "punto";

    public static string Method(PaymentMethod method) => method switch
    {
        PaymentMethod.Transfer => "transferencia",
        PaymentMethod.Card => "tarjeta",
        PaymentMethod.Credit => "credito",
        _ => "efectivo",
    };

    public static string Status(SaleStatus status) => status switch
    {
        SaleStatus.Pending => "preliminar",
        SaleStatus.Cancelled => "anulada",
        _ => "cerrada",
    };
}
