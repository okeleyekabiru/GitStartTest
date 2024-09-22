using Inventory.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.API.Domain.EnityConfiguration
{
    public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
    {
        public void Configure(EntityTypeBuilder<InventoryItem> builder)
        {
            builder.ToTable("InventoryItems");

            builder.HasKey(ii => ii.Id);

            builder.Property(ii => ii.ProductId)
                .IsRequired();

            builder.Property(ii => ii.QuantityInStock)
                .IsRequired();

            builder.Property(ii => ii.LastRestocked)
                .IsRequired();

            builder.Property(ii => ii.DateCreated)
                .IsRequired();

            builder.HasIndex(ii => ii.ProductId);
        }
    }
}