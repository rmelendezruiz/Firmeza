using Firmeza.Domain.Entities;
using Firmeza.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Firmeza.Infrastructure.Persistence.Configurations;

public sealed class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("ventas");
        builder.HasKey(sale => sale.Id);
        builder.Property(sale => sale.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(sale => sale.Number).HasColumnName("numero").HasMaxLength(20).IsRequired();
        builder.Property(sale => sale.Date).HasColumnName("fecha");
        builder.Property(sale => sale.CustomerId).HasColumnName("cliente_id");
        builder.Property(sale => sale.BranchId).HasColumnName("sede_id");
        builder.Property(sale => sale.Channel).HasColumnName("canal").HasConversion(StoredEnums.Channel).HasMaxLength(20);
        builder.Property(sale => sale.PaymentMethod).HasColumnName("metodo_pago").HasConversion(StoredEnums.Pay).HasMaxLength(20);
        builder.Property(sale => sale.CarId).HasColumnName("carro_id");
        builder.Property(sale => sale.CarPrice).HasColumnName("precio_carro").HasPrecision(14, 2);
        builder.Property(sale => sale.Status).HasColumnName("estado").HasConversion(StoredEnums.Status).HasMaxLength(20);
        builder.Property(sale => sale.InspectionDeposit).HasColumnName("cargo_preliminar").HasPrecision(14, 2);
        builder.Property(sale => sale.InspectionDeadline).HasColumnName("plazo_peritaje");
        builder.Property(sale => sale.SellerEmail).HasColumnName("vendedor_correo").HasMaxLength(160).IsRequired();
        builder.Ignore(sale => sale.Iva);
        builder.Ignore(sale => sale.Total);
        builder.Ignore(sale => sale.IsActive);
        builder.HasIndex(sale => sale.Number).IsUnique();
        builder.HasIndex(sale => sale.CarId)
            .IsUnique()
            .HasFilter("carro_id IS NOT NULL AND estado <> 'Anulada'");
        builder.HasIndex(sale => sale.Date);
        builder.HasIndex(sale => new { sale.BranchId, sale.Date });
        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(sale => sale.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Branch>()
            .WithMany()
            .HasForeignKey(sale => sale.BranchId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Car>()
            .WithMany()
            .HasForeignKey(sale => sale.CarId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(sale => sale.Lines)
            .WithOne()
            .HasForeignKey(line => line.SaleId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(sale => sale.Lines).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
