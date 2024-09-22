using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Product.API.Domain.Entities;

namespace Product.API.Infrastructure.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
        {
            public void Configure(EntityTypeBuilder<Category> builder)
            {
                builder.ToTable("Categories");

                builder.HasKey(c => c.Id);

                builder.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                builder.Property(c => c.Description)
                    .HasMaxLength(500);

                builder.Property(c => c.IsActive)
                    .IsRequired();

                // Create an index on the Name column for quick searches
                builder.HasIndex(c => c.Name);
            }
        }
    }