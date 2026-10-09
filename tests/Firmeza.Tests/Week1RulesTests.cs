using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Tests;

public class Week1RulesTests
{
    [Fact]
    public void Rechaza_correo_invalido()
    {
        var ex = Assert.Throws<BusinessRuleException>(() =>
            Customer.Create("Ana Ruiz", "ana-sin-arroba", "1234567890", "3001234567"));

        Assert.Contains("correo", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Rechaza_documento_que_no_cumple_el_formato()
    {
        var ex = Assert.Throws<BusinessRuleException>(() =>
            Customer.Create("Ana Ruiz", "ana@firmeza.local", "12", "3001234567"));

        Assert.Contains("documento", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Rechaza_telefono_invalido()
    {
        var ex = Assert.Throws<BusinessRuleException>(() =>
            Customer.Create("Ana Ruiz", "ana@firmeza.local", "1234567890", "123"));

        Assert.Contains("teléfono", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Acepta_cliente_con_correo_documento_y_telefono_validos()
    {
        var customer = Customer.Create("Ana Ruiz", "Ana@Firmeza.local", "1234567890", "3001234567");

        Assert.Equal("ana@firmeza.local", customer.Email);
        Assert.Equal("1234567890", customer.Document);
        Assert.Equal("3001234567", customer.Phone);
    }

    [Fact]
    public void Rechaza_precio_no_positivo()
    {
        var ex = Assert.Throws<BusinessRuleException>(() =>
            Product.Create("Filtro de aceite", 0, 10, "FIL-1", Guid.NewGuid(), Guid.NewGuid(), []));

        Assert.Contains("precio", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Rechaza_cantidad_negativa()
    {
        var ex = Assert.Throws<BusinessRuleException>(() =>
            Product.Create("Filtro de aceite", 25000, -1, "FIL-1", Guid.NewGuid(), Guid.NewGuid(), []));

        Assert.Contains("cantidad", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Venta_calcula_iva_y_total()
    {
        var sale = Sale.Register(
            "V-000001",
            DateTime.UtcNow,
            Guid.NewGuid(),
            Guid.NewGuid(),
            SaleChannel.PhysicalStore,
            PaymentMethod.Cash,
            null,
            0m,
            [(Guid.NewGuid(), 2, 1000m)]);

        Assert.Equal(380m, sale.Iva);
        Assert.Equal(2380m, sale.Total);
        Assert.Single(sale.Lines);
    }

    [Fact]
    public void Venta_rechaza_cantidad_en_cero()
    {
        var ex = Assert.Throws<BusinessRuleException>(() =>
            Sale.Register(
                "V-000001",
                DateTime.UtcNow,
                Guid.NewGuid(),
                Guid.NewGuid(),
                SaleChannel.PhysicalStore,
                PaymentMethod.Cash,
                null,
                0m,
                [(Guid.NewGuid(), 0, 1000m)]));

        Assert.Contains("cantidad", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}
