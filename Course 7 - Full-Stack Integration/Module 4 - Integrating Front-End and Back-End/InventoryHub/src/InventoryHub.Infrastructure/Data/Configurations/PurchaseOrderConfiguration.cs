using InventoryHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryHub.Infrastructure.Data.Configurations;

public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(o => o.TotalAmount)
            .HasPrecision(18, 2);

        builder.Property(o => o.Notes)
            .HasMaxLength(1000);

        builder.HasIndex(o => o.OrderNumber).IsUnique();
        builder.HasIndex(o => o.SupplierId);
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.OrderDate);

        builder.HasOne(o => o.Supplier)
            .WithMany(s => s.Orders)
            .HasForeignKey(o => o.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
