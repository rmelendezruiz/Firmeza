using Firmeza.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Firmeza.Infrastructure.Persistence.Configurations;

public sealed class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("vehiculos");
        builder.HasKey(vehicle => vehicle.Id);
        builder.Property(vehicle => vehicle.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(vehicle => vehicle.Brand).HasColumnName("marca").HasMaxLength(80).IsRequired();
        builder.Property(vehicle => vehicle.Line).HasColumnName("linea").HasMaxLength(80).IsRequired();
        builder.Property(vehicle => vehicle.Year).HasColumnName("anio");
        builder.HasIndex(vehicle => new { vehicle.Brand, vehicle.Line, vehicle.Year }).IsUnique();
    }
}
