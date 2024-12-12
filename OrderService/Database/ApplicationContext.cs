using System.Reflection;
using Microsoft.EntityFrameworkCore;
using OrderService.Entities;

namespace OrderService.Database
{
    public class ApplicationContext(DbContextOptions options) : DbContext(options)
    {

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public DbSet<Order> Orders { get; init; }
        public DbSet<OrderItem> OrderItems { get; init; }
        public DbSet<Payment> Payments { get; init; }
        public DbSet<OrderStateMachineData> SagaData { get; init; }
    }
}
