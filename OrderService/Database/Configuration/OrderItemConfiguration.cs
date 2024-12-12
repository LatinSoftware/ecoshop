using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Entities;

namespace OrderService.Database.Configuration
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrderId).HasConversion(orderId => orderId.Value, id => new OrderId(id));

            builder.Property(x => x.ProductId).HasConversion(productId => productId.Value, id => new ProductId(id));

            builder.ComplexProperty(x => x.Quantity, x =>
            {
                x.Property(x => x.Value).HasColumnName(nameof(OrderItem.Quantity));
            });

            builder.ComplexProperty(x => x.Price, x =>
            {
                x.Property(o => o.Value).HasPrecision(10, 2).HasColumnName(nameof(OrderItem.Price));
            });

            builder.Ignore(x => x.Total);

            builder
            .HasOne(o => o.Order)
            .WithMany(o => o.Items)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
