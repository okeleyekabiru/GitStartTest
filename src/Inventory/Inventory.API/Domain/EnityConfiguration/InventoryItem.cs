using Inventory.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.API.Domain.EnityConfiguration
{
    public class StockTransactionConfiguration : IEntityTypeConfiguration<StockTransaction>
    {
        public void Configure(EntityTypeBuilder<StockTransaction> builder)
        {
            builder.ToTable("StockTransactions");

            builder.HasKey(st => st.Id);

            builder.Property(st => st.InventoryItemId)
                .IsRequired();

            builder.Property(st => st.QuantityChanged)
                .IsRequired();

            builder.Property(st => st.DateOfTransaction)
                .IsRequired();

            builder.Property(st => st.TransactionType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(st => st.Reason)
                .HasMaxLength(200);

            builder.HasIndex(st => st.InventoryItemId);
        }
    }
}