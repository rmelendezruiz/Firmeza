using Firmeza.Domain.Entities;
using Firmeza.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Firmeza.Infrastructure.Persistence.Configurations;

public sealed class BranchAssetConfiguration : IEntityTypeConfiguration<BranchAsset>
{
    public void Configure(EntityTypeBuilder<BranchAsset> builder)
    {
        builder.ToTable("activos_sede");
        builder.HasKey(asset => asset.Id);
        builder.Property(asset => asset.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(asset => asset.BranchId).HasColumnName("sede_id");
        builder.Property(asset => asset.Name).HasColumnName("nombre").HasMaxLength(120).IsRequired();
        builder.Property(asset => asset.Type).HasColumnName("tipo").HasConversion(StoredEnums.Asset).HasMaxLength(20);
        builder.Property(asset => asset.Quantity).HasColumnName("cantidad");
        builder.Property(asset => asset.Status).HasColumnName("estado").HasConversion(StoredEnums.AssetState).HasMaxLength(20);
        builder.HasIndex(asset => new { asset.BranchId, asset.Name }).IsUnique();
    }
}
