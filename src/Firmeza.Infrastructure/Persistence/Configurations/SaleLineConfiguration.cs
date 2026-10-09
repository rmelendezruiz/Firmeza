using Firmeza.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Firmeza.Infrastructure.Persistence.Configurations;

public sealed class SaleLineConfiguration : IEntityTypeConfiguration<SaleLine>
{
    public void Configure(EntityTypeBuilder<SaleLine> builder)
    {
        builder.ToTable("detalles_venta");
        builder.HasKey(line => line.Id);
        builder.Property(line => line.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(line => line.SaleId).HasColumnName("venta_id");
        builder.Property(line => line.ProductId).HasColumnName("producto_id");
        builder.Property(line => line.Quantity).HasColumnName("cantidad");
        builder.Property(line => line.Price).HasColumnName("precio").HasPrecision(12, 2);
        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(line => line.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
