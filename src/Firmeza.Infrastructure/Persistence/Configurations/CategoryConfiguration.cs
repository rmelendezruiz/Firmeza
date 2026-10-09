using Firmeza.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Firmeza.Infrastructure.Persistence.Configurations;

public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categorias");
        builder.HasKey(category => category.Id);
        builder.Property(category => category.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(category => category.Name).HasColumnName("nombre").HasMaxLength(120).IsRequired();
        builder.HasIndex(category => category.Name).IsUnique();
    }
}
