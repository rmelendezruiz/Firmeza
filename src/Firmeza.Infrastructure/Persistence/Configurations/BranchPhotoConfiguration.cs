using Firmeza.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Firmeza.Infrastructure.Persistence.Configurations;

public sealed class BranchPhotoConfiguration : IEntityTypeConfiguration<BranchPhoto>
{
    public void Configure(EntityTypeBuilder<BranchPhoto> builder)
    {
        builder.ToTable("fotos_sede");
        builder.HasKey(foto => foto.Id);
        builder.Property(foto => foto.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(foto => foto.BranchId).HasColumnName("sede_id");
        builder.Property(foto => foto.Url).HasColumnName("url").HasMaxLength(300).IsRequired();
        builder.Property(foto => foto.Description).HasColumnName("descripcion").HasMaxLength(160).IsRequired();
        builder.Property(foto => foto.Sort).HasColumnName("orden");
        builder.HasIndex(foto => new { foto.BranchId, foto.Sort }).IsUnique();
    }
}
