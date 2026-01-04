using InventoryHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryHub.Infrastructure.Data.Configurations;

public class SalesOrderConfiguration : IEntityTypeConfiguration<SalesOrder>
{
    public void Configure(EntityTypeBuilder<SalesOrder> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(o => o.TotalAmount)
            .HasPrecision(18, 2);

        builder.Property(o => o.ShippingAddress)
            .HasMaxLength(500);

        builder.Property(o => o.Notes)
            .HasMaxLength(1000);

        builder.HasIndex(o => o.OrderNumber).IsUnique();
        builder.HasIndex(o => o.ClientId);
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.OrderDate);

        builder.HasOne(o => o.Client)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
