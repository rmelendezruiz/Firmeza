using Firmeza.Application.Staff;
using Firmeza.Application.Identity;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Rules;

namespace Firmeza.Tests;

public class BranchRulesTests
{
    private static Sale CompraUsada(DateTime date)
    {
        var sale = Sale.Register(
            "V-000010",
            date,
            Guid.NewGuid(),
            Guid.NewGuid(),
            SaleChannel.Web,
            PaymentMethod.Card,
            Guid.NewGuid(),
            50_000_000m,
            []);
        sale.OpenInspection(Inspection.Deposit(50_000_000m), date.AddDays(Inspection.Days));
        return sale;
    }

    [Fact]
    public void La_pagina_no_acepta_efectivo()
    {
        var ex = Assert.Throws<BusinessRuleException>(() =>
            Sale.Register(
                "V-000011",
                DateTime.UtcNow,
                Guid.NewGuid(),
                Guid.NewGuid(),
                SaleChannel.Web,
                PaymentMethod.Cash,
                Guid.NewGuid(),
                1000m,
                []));

        Assert.Contains("página", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Venta_sin_counter_es_rechazada()
    {
        var ex = Assert.Throws<BusinessRuleException>(() =>
            Sale.Register(
                "V-000012",
                DateTime.UtcNow,
                Guid.NewGuid(),
                Guid.Empty,
                SaleChannel.PhysicalStore,
                PaymentMethod.Cash,
                Guid.NewGuid(),
                1000m,
                []));

        Assert.Contains("counter", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void La_compra_usada_queda_en_peritaje_con_el_diez_por_ciento()
    {
        var sale = CompraUsada(DateTime.UtcNow);

        Assert.Equal(SaleStatus.Pending, sale.Status);
        Assert.Equal(5_000_000m, sale.InspectionDeposit);
        Assert.True(sale.IsActive);
    }

    [Fact]
    public void Cerrar_un_peritaje_vigente_cierra_la_venta()
    {
        var ahora = DateTime.UtcNow;
        var sale = CompraUsada(ahora);

        sale.CloseInspection(ahora.AddDays(2));

        Assert.Equal(SaleStatus.Closed, sale.Status);
    }

    [Fact]
    public void Un_peritaje_vencido_no_se_cierra()
    {
        var ahora = DateTime.UtcNow;
        var sale = CompraUsada(ahora.AddDays(-8));

        Assert.True(sale.InspectionExpired(ahora));
        Assert.Throws<BusinessRuleException>(() => sale.CloseInspection(ahora));
    }

    [Fact]
    public void Anular_saca_la_venta_de_las_activas()
    {
        var sale = CompraUsada(DateTime.UtcNow);

        sale.Cancel();

        Assert.Equal(SaleStatus.Cancelled, sale.Status);
        Assert.False(sale.IsActive);
    }

    [Fact]
    public void Una_venta_cerrada_no_se_anula()
    {
        var sale = Sale.Register(
            "V-000013",
            DateTime.UtcNow,
            Guid.NewGuid(),
            Guid.NewGuid(),
            SaleChannel.PhysicalStore,
            PaymentMethod.Credit,
            Guid.NewGuid(),
            1000m,
            []);

        Assert.Throws<BusinessRuleException>(sale.Cancel);
    }

    [Fact]
    public void El_metodo_de_pago_acepta_credito_con_tilde()
    {
        Assert.Equal(PaymentMethod.Credit, Sale.FromPaymentMethod(" Crédito "));
        Assert.Throws<BusinessRuleException>(() => Sale.FromPaymentMethod("cheque"));
    }

    [Fact]
    public void Carro_sin_counter_es_rechazado()
    {
        var ex = Assert.Throws<BusinessRuleException>(() =>
            Car.Create("Kia", "Rio", 2019, "automovil", "usado", 60000, "ambos", 45_000_000m, 47_000_000m, "", Guid.Empty));

        Assert.Contains("counter", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void La_ficha_sin_counter_queda_vacia()
    {
        var card = StaffCard.Create("Comercial@Firmeza.local", Guid.Empty);

        Assert.Equal("comercial@firmeza.local", card.Email);
        Assert.Null(card.BranchId);
    }

    [Fact]
    public void El_admin_heredado_entra_y_el_cliente_no_es_equipo()
    {
        Assert.Equal(Roles.Admin, Roles.FirstStaffRole(["Administrador"]));
        Assert.Null(Roles.FirstStaffRole([Roles.Customer]));
        Assert.Equal(Roles.Customer, Roles.SessionRole([Roles.Customer]));
    }

    [Fact]
    public void El_comercial_sin_counter_no_vende_ni_ve_otros_counters()
    {
        var profile = new StaffSessionProfile("comercial@firmeza.local", Roles.Salesperson, null);

        Assert.Throws<BusinessRuleException>(() => profile.OwnBranch());
        Assert.Equal(Guid.Empty, profile.Scope);
        Assert.False(profile.Reaches(Guid.NewGuid()));
    }

    [Fact]
    public void El_gerente_de_firmeza_alcanza_todos_los_counters()
    {
        var profile = new StaffSessionProfile("direccion@firmeza.local", Roles.CompanyManager, null);

        Assert.Null(profile.Scope);
        Assert.True(profile.Reaches(Guid.NewGuid()));
    }

    [Fact]
    public void El_gerente_solo_alcanza_su_counter()
    {
        var baq = Guid.NewGuid();
        var profile = new StaffSessionProfile("gerente@firmeza.local", Roles.Manager, baq);

        Assert.True(profile.Reaches(baq));
        Assert.False(profile.Reaches(Guid.NewGuid()));
        Assert.Equal(baq, profile.OwnBranch());
    }
}
