using Firmeza.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Firmeza.Infrastructure.Persistence.Configurations;

public sealed class SalesTargetConfiguration : IEntityTypeConfiguration<SalesTarget>
{
    public void Configure(EntityTypeBuilder<SalesTarget> builder)
    {
        builder.ToTable("metas");
        builder.HasKey(target => target.Id);
        builder.Property(target => target.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(target => target.Email).HasColumnName("correo").HasMaxLength(160).IsRequired();
        builder.Property(target => target.Period).HasColumnName("periodo").HasMaxLength(7).IsRequired();
        builder.Property(target => target.Amount).HasColumnName("monto").HasPrecision(14, 2);
        builder.HasIndex(target => new { target.Email, target.Period }).IsUnique();
    }
}
