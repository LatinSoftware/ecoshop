using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Entities;

namespace OrderService.Database.Configuration
{
    public class SagaDataConfiguration : IEntityTypeConfiguration<OrderStateMachineData>
    {
        public void Configure(EntityTypeBuilder<OrderStateMachineData> builder)
        {
            builder.HasKey(x => x.CorrelationId);
        }
    }
}
