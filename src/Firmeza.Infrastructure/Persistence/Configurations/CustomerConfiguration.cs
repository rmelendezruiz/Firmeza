using Firmeza.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Firmeza.Infrastructure.Persistence.Configurations;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("clientes");
        builder.HasKey(customer => customer.Id);
        builder.Property(customer => customer.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(customer => customer.Name).HasColumnName("nombre").HasMaxLength(120).IsRequired();
        builder.Property(customer => customer.Email).HasColumnName("correo").HasMaxLength(160).IsRequired();
        builder.Property(customer => customer.Document).HasColumnName("documento").HasMaxLength(12).IsRequired();
        builder.Property(customer => customer.Phone).HasColumnName("telefono").HasMaxLength(15).IsRequired();
        builder.HasIndex(customer => customer.Email).IsUnique();
        builder.HasIndex(customer => customer.Document).IsUnique();
    }
}
