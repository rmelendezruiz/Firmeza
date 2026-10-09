using Firmeza.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Firmeza.Infrastructure.Persistence.Configurations;

public sealed class StaffCardConfiguration : IEntityTypeConfiguration<StaffCard>
{
    public void Configure(EntityTypeBuilder<StaffCard> builder)
    {
        builder.ToTable("fichas_equipo");
        builder.HasKey(card => card.Id);
        builder.Property(card => card.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(card => card.Email).HasColumnName("correo").HasMaxLength(160).IsRequired();
        builder.Property(card => card.BranchId).HasColumnName("sede_id");
        builder.Property(card => card.Salary).HasColumnName("sueldo").HasPrecision(14, 2);
        builder.Property(card => card.CommissionPercent).HasColumnName("porcentaje_comision").HasPrecision(5, 2);
        builder.HasIndex(card => card.Email).IsUnique();
        builder.HasOne<Branch>()
            .WithMany()
            .HasForeignKey(card => card.BranchId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
