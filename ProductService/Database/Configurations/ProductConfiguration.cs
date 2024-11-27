using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Entities;

namespace ProductService.Database.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(p => p.Id).HasConversion(id => id.Value, comparer => new ProductId(comparer));
            builder.Property(x => x.Name).IsRequired().HasMaxLength(500);
            builder.Property(x => x.Price).IsRequired().HasPrecision(10,2);
            builder.Property(x => x.Description).HasMaxLength(1500);

            builder.Property(x => x.StockQuantity).IsRequired().HasConversion(qty => qty.Value, comparer => new StockQuantity(comparer));
            builder.Property(x => x.ReservedQuantity).IsRequired().HasConversion(qty => qty.Value, comparer => new ReservedQuantity(comparer));

            builder.HasOne(x => x.Category).WithMany(x => x.Products)
                .HasForeignKey(x => x.CategoryId)
                .IsRequired();

        }
    }
}
