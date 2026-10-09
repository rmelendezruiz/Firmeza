using Firmeza.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Firmeza.Infrastructure.Persistence.Configurations;

public sealed class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable("marcas");
        builder.HasKey(brand => brand.Id);
        builder.Property(brand => brand.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(brand => brand.Name).HasColumnName("nombre").HasMaxLength(120).IsRequired();
        builder.HasIndex(brand => brand.Name).IsUnique();
    }
}
