using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Entities;

namespace OrderService.Database.Configuration
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(x => x.OrderId);
            builder.Property(x => x.OrderId).HasConversion(orderId => orderId.Value, id => new OrderId(id));
            builder.Property(x => x.UserId).HasConversion(orderId => orderId.Value, id => new UserId(id));
            builder.Property(x => x.Subtotal).HasPrecision(10, 2);
            builder.Property(x => x.Taxes).HasPrecision(10, 2);
            builder.Ignore(x => x.Total);
            builder.Property(x => x.Status).HasConversion<string>();
            builder.Property(x => x.CreatedAt);
            builder.Property(x => x.LastUpdatedAt);

            builder
                .HasMany(o => o.Items)
                .WithOne(i => i.Order)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(o => o.Payment).WithOne(p => p.Order).HasForeignKey<Payment>(p => p.OrderId).IsRequired();


        }
    }
}
