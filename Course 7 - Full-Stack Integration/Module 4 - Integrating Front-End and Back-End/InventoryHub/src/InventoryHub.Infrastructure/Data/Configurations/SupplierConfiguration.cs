using InventoryHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryHub.Infrastructure.Data.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.ContactPerson)
            .HasMaxLength(100);

        builder.Property(s => s.Email)
            .HasMaxLength(100);

        builder.Property(s => s.Phone)
            .HasMaxLength(50);

        builder.Property(s => s.Address)
            .HasMaxLength(500);

        builder.HasIndex(s => s.Name);
        builder.HasIndex(s => s.IsActive);
    }
}
