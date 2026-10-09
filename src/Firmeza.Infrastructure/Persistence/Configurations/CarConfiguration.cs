using Firmeza.Domain.Entities;
using Firmeza.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Firmeza.Infrastructure.Persistence.Configurations;

public sealed class CarConfiguration : IEntityTypeConfiguration<Car>
{
    public void Configure(EntityTypeBuilder<Car> builder)
    {
        builder.ToTable("carros");
        builder.HasKey(car => car.Id);
        builder.Property(car => car.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(car => car.Brand).HasColumnName("marca").HasMaxLength(80).IsRequired();
        builder.Property(car => car.Line).HasColumnName("linea").HasMaxLength(80).IsRequired();
        builder.Property(car => car.Year).HasColumnName("anio");
        builder.Property(car => car.Type).HasColumnName("tipo").HasConversion(StoredEnums.Type).HasMaxLength(20);
        builder.Property(car => car.Condition).HasColumnName("condicion").HasConversion(StoredEnums.Condition).HasMaxLength(20);
        builder.Property(car => car.Mileage).HasColumnName("kilometraje");
        builder.Property(car => car.Availability).HasColumnName("disponibilidad").HasConversion(StoredEnums.Availability).HasMaxLength(20);
        builder.Property(car => car.PagePrice).HasColumnName("precio_pagina").HasPrecision(12, 2);
        builder.Property(car => car.CounterPrice).HasColumnName("precio_counter").HasPrecision(12, 2);
        builder.Property(car => car.PhotoUrl).HasColumnName("foto_url").HasMaxLength(300).IsRequired();
        builder.Property(car => car.BranchId).HasColumnName("sede_id");
        builder.Property(car => car.Sold).HasColumnName("vendido");
        builder.Ignore(car => car.CanBuyOnSite);
        builder.HasOne<Branch>()
            .WithMany()
            .HasForeignKey(car => car.BranchId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
