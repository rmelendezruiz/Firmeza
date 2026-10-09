using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Products;

public sealed class DeleteProduct(
    IProductStore products,
    ISaleStore sales,
    IUnitOfWork work)
{
    public async Task ExecuteAsync(Guid id, CancellationToken ct)
    {
        var product = await products.GetAsync(id, ct)
            ?? throw new NotFoundException("Producto no encontrado.");
        if (await sales.ProductHasSalesAsync(id, ct))
        {
            throw new BusinessRuleException("El producto está en ventas y no se puede eliminar.");
        }

        await products.RemoveAsync(product, ct);
        await work.SaveAsync(ct);
    }
}
