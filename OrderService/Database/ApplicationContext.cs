using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using OrderService.Entities;

namespace OrderService.Database
{
    public class ApplicationContext(DbContextOptions options) : DbContext(options)
    {
        public static ApplicationContext Create(IMongoDatabase database) => new(new DbContextOptionsBuilder<ApplicationContext>()
            .UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName)
            .Options);


        public DbSet<Order> Orders { get; init; }
        public DbSet<OrderItem> OrderItems { get; init; }
        public DbSet<PaymentInfo> PaymentInfos { get; init; }
    }
}
