using Firmeza.Application.Staff;
using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Sales;

public sealed class DeleteSale(
    StaffSession session,
    ISaleStore sales,
    IProductStore products,
    ICarStore cars,
    IUnitOfWork work)
{
    public async Task ExecuteAsync(Guid id, CancellationToken ct)
    {
        var yo = await session.RequireAsync(ct);
        var sale = await sales.GetAsync(id, ct);
        if (sale is null || !yo.Reaches(sale.BranchId))
        {
            throw new NotFoundException("Venta no encontrada.");
        }

        if (sale.Channel == SaleChannel.Web && sale.SellerEmail.Length == 0)
        {
            throw new BusinessRuleException("La compra que hizo el cliente en la página no se elimina. Si el peritaje no pasa, se anula.");
        }

        foreach (var line in sale.Lines)
        {
            var product = await products.GetAsync(line.ProductId, ct)
                ?? throw new NotFoundException("Producto no encontrado.");
            product.Restock(line.Quantity);
        }

        if (sale.IsActive)
        {
            await CancelSale.Release(sale, cars, ct);
        }

        await sales.RemoveAsync(sale, ct);
        await work.SaveAsync(ct);
    }
}
