using Firmeza.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Firmeza.Infrastructure.Persistence.Configurations;

public sealed class StaffMessageConfiguration : IEntityTypeConfiguration<StaffMessage>
{
    public void Configure(EntityTypeBuilder<StaffMessage> builder)
    {
        builder.ToTable("mensajes");
        builder.HasKey(message => message.Id);
        builder.Property(message => message.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(message => message.FromEmail).HasColumnName("de_correo").HasMaxLength(160).IsRequired();
        builder.Property(message => message.ToEmail).HasColumnName("para_correo").HasMaxLength(160).IsRequired();
        builder.Property(message => message.Text).HasColumnName("texto").HasMaxLength(500).IsRequired();
        builder.Property(message => message.Date).HasColumnName("fecha");
        builder.HasIndex(message => message.FromEmail);
        builder.HasIndex(message => message.ToEmail);
    }
}
