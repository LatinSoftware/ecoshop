using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Entities;

namespace OrderService.Database.Configuration
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(x => x.PaymentId);
            builder.Property(x => x.OrderId).HasConversion(orderId => orderId.Value, id => new OrderId(id));
            builder.Property(x => x.Status).HasConversion<string>();
            builder.Property(x => x.Method).HasConversion<string>();
            builder.Property(x => x.TransactionId).IsRequired(false);
            builder.Property(x => x.CreatedAt).ValueGeneratedOnAdd();

            builder.HasOne(p => p.Order).WithOne(o => o.Payment).HasForeignKey<Payment>(p => p.OrderId).IsRequired();
        }
    }
}
