using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Tests;

public class CatalogRulesTests
{
    [Fact]
    public void Normaliza_el_sku_y_permite_un_repuesto_sin_vehiculo()
    {
        var product = Product.Create(
            "Filtro de aceite",
            45000.50m,
            4,
            "fil-001",
            Guid.NewGuid(),
            Guid.NewGuid(),
            []);

        Assert.Equal("FIL-001", product.Sku);
        Assert.Empty(product.Fitments);
    }

    [Fact]
    public void Rechaza_sku_invalido()
    {
        var ex = Assert.Throws<BusinessRuleException>(() =>
            Product.Create("Filtro", 1000, 1, "filtro aceite", Guid.NewGuid(), Guid.NewGuid(), []));

        Assert.Contains("SKU", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Rechaza_producto_sin_marca()
    {
        var ex = Assert.Throws<BusinessRuleException>(() =>
            Product.Create("Filtro", 1000, 1, "FIL-1", Guid.Empty, Guid.NewGuid(), []));

        Assert.Contains("marca", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Rechaza_el_mismo_vehiculo_dos_veces()
    {
        var vehicleId = Guid.NewGuid();
        var ex = Assert.Throws<BusinessRuleException>(() =>
            Product.Create("Pastilla", 80000, 2, "FRN-2", Guid.NewGuid(), Guid.NewGuid(), [vehicleId, vehicleId]));

        Assert.Contains("vehículo", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Rechaza_anio_de_vehiculo_fuera_de_rango()
    {
        var ex = Assert.Throws<BusinessRuleException>(() => Vehicle.Create("Toyota", "Corolla", 1800));

        Assert.Contains("año", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Venta_de_carro_en_la_pagina_calcula_iva()
    {
        var sale = Sale.Register(
            "V-000002",
            DateTime.UtcNow,
            Guid.NewGuid(),
            Guid.NewGuid(),
            SaleChannel.Web,
            PaymentMethod.Transfer,
            Guid.NewGuid(),
            1000m,
            []);

        Assert.Equal(SaleChannel.Web, sale.Channel);
        Assert.Equal(190m, sale.Iva);
        Assert.Equal(1190m, sale.Total);
        Assert.Empty(sale.Lines);
    }

    [Fact]
    public void Venta_rechaza_carro_y_repuestos_en_el_mismo_registro()
    {
        var ex = Assert.Throws<BusinessRuleException>(() =>
            Sale.Register(
                "V-000003",
                DateTime.UtcNow,
                Guid.NewGuid(),
                Guid.NewGuid(),
                SaleChannel.PhysicalStore,
                PaymentMethod.Cash,
                Guid.NewGuid(),
                1000m,
                [(Guid.NewGuid(), 1, 500m)]));

        Assert.Contains("separadas", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Acepta_vehiculo_con_marca_linea_y_anio()
    {
        var vehicle = Vehicle.Create(" Toyota ", "Corolla", 2020);

        Assert.Equal("Toyota", vehicle.Brand);
        Assert.Equal("Corolla", vehicle.Line);
        Assert.Equal(2020, vehicle.Year);
    }
}
