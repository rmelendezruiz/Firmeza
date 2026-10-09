using Firmeza.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Firmeza.Infrastructure.Persistence.Configurations;

public sealed class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("sedes");
        builder.HasKey(branch => branch.Id);
        builder.Property(branch => branch.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(branch => branch.Code).HasColumnName("codigo").HasMaxLength(3).IsRequired();
        builder.Property(branch => branch.Name).HasColumnName("nombre").HasMaxLength(80).IsRequired();
        builder.Property(branch => branch.City).HasColumnName("ciudad").HasMaxLength(80).IsRequired();
        builder.Property(branch => branch.Headquarters).HasColumnName("direccion").HasMaxLength(160).IsRequired();
        builder.Property(branch => branch.Phone).HasColumnName("telefono").HasMaxLength(20).IsRequired();
        builder.Property(branch => branch.InspectionPhone).HasColumnName("telefono_peritaje").HasMaxLength(20).IsRequired();
        builder.Property(branch => branch.Email).HasColumnName("correo").HasMaxLength(160).IsRequired();
        builder.Property(branch => branch.Hours).HasColumnName("horario").HasMaxLength(120).IsRequired();
        builder.Property(branch => branch.Latitude).HasColumnName("latitud").HasPrecision(9, 6);
        builder.Property(branch => branch.Longitude).HasColumnName("longitud").HasPrecision(9, 6);
        builder.Property(branch => branch.Capacity).HasColumnName("capacidad");
        builder.Property(branch => branch.AreaM2).HasColumnName("area_m2");
        builder.HasIndex(branch => branch.Code).IsUnique();
        builder.HasIndex(branch => branch.City);
        builder.HasMany(branch => branch.Photos)
            .WithOne()
            .HasForeignKey(foto => foto.BranchId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(branch => branch.Assets)
            .WithOne()
            .HasForeignKey(asset => asset.BranchId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(branch => branch.Photos).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(branch => branch.Assets).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
