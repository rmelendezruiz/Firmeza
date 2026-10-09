using Firmeza.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Firmeza.Infrastructure.Persistence.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("productos");
        builder.HasKey(product => product.Id);
        builder.Property(product => product.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(product => product.Sku).HasColumnName("sku").HasMaxLength(32).IsRequired();
        builder.Property(product => product.Name).HasColumnName("nombre").HasMaxLength(120).IsRequired();
        builder.Property(product => product.Price).HasColumnName("precio").HasPrecision(12, 2);
        builder.Property(product => product.Quantity).HasColumnName("cantidad");
        builder.Property(product => product.BrandId).HasColumnName("marca_id");
        builder.Property(product => product.CategoryId).HasColumnName("categoria_id");
        builder.HasIndex(product => product.Sku).IsUnique();
        builder.HasOne<Brand>()
            .WithMany()
            .HasForeignKey(product => product.BrandId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(product => product.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(product => product.Fitments)
            .WithOne()
            .HasForeignKey(fitment => fitment.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(product => product.Fitments)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .AutoInclude();
    }
}
