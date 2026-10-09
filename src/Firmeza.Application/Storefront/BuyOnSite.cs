using System.Globalization;
using Firmeza.Application.Customers;
using Firmeza.Application.Errors;
using Firmeza.Application.Identity;
using Firmeza.Application.Ports;
using Firmeza.Application.Session;
using Firmeza.Application.Sales;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Application.Storefront;

public sealed class BuyOnSite(
    IAdminIdentity identity,
    ICustomerStore customers,
    ICarStore cars,
    IBranchStore branches,
    ISaleStore sales,
    IUnitOfWork work)
{
    public async Task<SitePurchaseDto> ExecuteAsync(BuyOnSiteCommand command, CancellationToken ct)
    {
        var card = await SessionCustomer.RequireAsync(identity, customers, ct);
        if (!command.AcceptsTerms)
        {
            throw new BusinessRuleException("Debes aceptar los términos de la compra.");
        }

        var metodo = Sale.FromPaymentMethod(command.PaymentMethod);
        if (await sales.CustomerHasInspectionAsync(card.Id, ct))
        {
            throw new ConflictException("Ya tienes un carro reservado en peritaje. Espera a que se cierre o se anule antes de reservar otro.");
        }

        var car = await cars.GetAsync(command.CarId, ct)
            ?? throw new NotFoundException("Carro no encontrado.");
        if (car.Sold)
        {
            throw new ConflictException("Ese carro ya se vendió.");
        }

        var price = car.PriceOn(SaleChannel.Web);
        if (price != command.SeenPrice)
        {
            var text = price.ToString("#,0", CultureInfo.InvariantCulture).Replace(',', '.');
            throw new ConflictException($"El precio cambió: ahora es $ {text}. Revísalo y confirma otra vez.");
        }

        var branch = await branches.GetAsync(car.BranchId, ct)
            ?? throw new NotFoundException("Counter no encontrado.");
        car.Sell();
        var ahora = DateTime.UtcNow;
        var sale = Sale.Register(
            SaleNumber.Next(await sales.LastNumberAsync(ct)),
            ahora,
            card.Id,
            branch.Id,
            SaleChannel.Web,
            metodo,
            car.Id,
            price,
            []);
        if (car.Condition == CarCondition.Used)
        {
            sale.OpenInspection(Inspection.Deposit(price), ahora.AddDays(Inspection.Days));
        }

        await sales.AddAsync(sale, ct);
        await work.SaveAsync(ct);
        return new SitePurchaseDto(
            sale.Number,
            SaleNames.CarName(car),
            sale.Total,
            sale.InspectionDeposit,
            sale.InspectionDeadline,
            SaleText.Status(sale.Status),
            SaleText.Method(sale.PaymentMethod),
            branch.Name,
            branch.InspectionPhone);
    }
}
