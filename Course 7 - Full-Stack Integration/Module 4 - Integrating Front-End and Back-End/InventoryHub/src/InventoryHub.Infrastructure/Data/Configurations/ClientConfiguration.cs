using InventoryHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryHub.Infrastructure.Data.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.ContactPerson)
            .HasMaxLength(100);

        builder.Property(c => c.Email)
            .HasMaxLength(100);

        builder.Property(c => c.Phone)
            .HasMaxLength(50);

        builder.Property(c => c.Address)
            .HasMaxLength(500);

        builder.Property(c => c.ShippingAddress)
            .HasMaxLength(500);

        builder.HasIndex(c => c.Name);
        builder.HasIndex(c => c.IsActive);
    }
}
