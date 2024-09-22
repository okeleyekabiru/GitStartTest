using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Product.API.Domain.EntityConfigurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Entities.Product>
    {
        public void Configure(EntityTypeBuilder<Entities.Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Description)
                .HasMaxLength(500);

            builder.Property(p => p.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.StockQuantity)
                .IsRequired();

            builder.Property(p => p.IsActive)
                .IsRequired();

            builder.Property(p => p.DateCreated)
                .IsRequired();

            builder.Property(p => p.DateUpdated)
                .IsRequired();

            // Create an index on the Name column for quick searches
            builder.HasIndex(p => p.Name);
        }
    }
}