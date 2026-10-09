using Firmeza.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Firmeza.Infrastructure.Persistence.Configurations;

public sealed class FitmentConfiguration : IEntityTypeConfiguration<Fitment>
{
    public void Configure(EntityTypeBuilder<Fitment> builder)
    {
        builder.ToTable("aplicaciones");
        builder.HasKey(fitment => fitment.Id);
        builder.Property(fitment => fitment.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(fitment => fitment.ProductId).HasColumnName("producto_id");
        builder.Property(fitment => fitment.VehicleId).HasColumnName("vehiculo_id");
        builder.HasIndex(fitment => new { fitment.ProductId, fitment.VehicleId }).IsUnique();
        builder.HasOne<Vehicle>()
            .WithMany()
            .HasForeignKey(fitment => fitment.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
