using InventoryHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryHub.Infrastructure.Data.Configurations;

public class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Reference)
            .HasMaxLength(100);

        builder.Property(m => m.Notes)
            .HasMaxLength(500);

        builder.Property(m => m.CreatedBy)
            .HasMaxLength(100);

        builder.HasIndex(m => m.ProductId);
        builder.HasIndex(m => m.CreatedAt);
        builder.HasIndex(m => m.Type);

        builder.HasOne(m => m.Product)
            .WithMany(p => p.Movements)
            .HasForeignKey(m => m.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
