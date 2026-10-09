using Firmeza.Application.Staff;
using Firmeza.Application.Errors;
using Firmeza.Application.Ports;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Sales;

public sealed class RegisterSale(
    StaffSession session,
    ISaleStore sales,
    ICustomerStore customers,
    IProductStore products,
    ICarStore cars,
    IBranchStore branches,
    IUnitOfWork work)
{
    public async Task<SaleDto> ExecuteAsync(RegisterSaleCommand command, CancellationToken ct)
    {
        var yo = await session.RequireAsync(ct);
        var branch = await branches.GetAsync(yo.OwnBranch(), ct)
            ?? throw new NotFoundException("Counter no encontrado.");
        var customer = await customers.GetAsync(command.CustomerId, ct)
            ?? throw new NotFoundException("Cliente no encontrado.");
        var channel = Channel(command.Channel);
        var metodo = Sale.FromPaymentMethod(command.PaymentMethod);
        var names = new Dictionary<Guid, string>();
        var lines = new List<(Guid ProductId, int Quantity, decimal Price)>();
        Car? car = null;

        decimal carPrice = 0;
        if (command.CarId is not null && command.CarId != Guid.Empty)
        {
            car = await cars.GetAsync(command.CarId.Value, ct)
                ?? throw new NotFoundException("Carro no encontrado.");
            if (car.BranchId != branch.Id)
            {
                throw new BusinessRuleException("Ese carro está en otro counter.");
            }

            if (car.Sold)
            {
                throw new ConflictException("Ese carro ya se vendió.");
            }

            carPrice = car.PriceOn(channel);
            car.Sell();
        }
        else
        {
            var seen = new HashSet<Guid>();
            foreach (var line in command.Lines ?? [])
            {
                if (!seen.Add(line.ProductId))
                {
                    throw new BusinessRuleException("La venta repite un producto.");
                }

                var product = await products.GetAsync(line.ProductId, ct)
                    ?? throw new NotFoundException("Producto no encontrado.");
                product.Deduct(line.Quantity);
                lines.Add((product.Id, line.Quantity, product.Price));
                names[product.Id] = product.Name;
            }
        }

        var sale = Sale.Register(
            SaleNumber.Next(await sales.LastNumberAsync(ct)),
            DateTime.UtcNow,
            customer.Id,
            branch.Id,
            channel,
            metodo,
            car?.Id,
            carPrice,
            lines);
        sale.AssignSeller(yo.Email);

        await sales.AddAsync(sale, ct);
        await work.SaveAsync(ct);
        return SaleDto.From(
            sale,
            customer.Name,
            branch.Name,
            names,
            car is null ? null : SaleNames.CarName(car));
    }

    private static SaleChannel Channel(string? channel)
    {
        return channel?.Trim().ToLowerInvariant() switch
        {
            null or "" or "punto" or "fisico" => SaleChannel.PhysicalStore,
            "pagina" => SaleChannel.Web,
            _ => throw new BusinessRuleException("El canal de la venta no es válido."),
        };
    }
}
