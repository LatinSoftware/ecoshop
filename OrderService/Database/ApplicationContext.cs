using MongoDB.Driver;
using OrderService.Entities;

namespace OrderService.Database
{
    public class ApplicationContext
    {
        private readonly IMongoDatabase database;
        public ApplicationContext(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<Order> Orders => database.GetCollection<Order>(nameof(Order).ToLowerInvariant());
        public IMongoCollection<OrderItem> OrderItems => database.GetCollection<OrderItem>(nameof(OrderItem).ToLowerInvariant());
        public IMongoCollection<PaymentInfo> PaymentInfos => database.GetCollection<PaymentInfo>(nameof(PaymentInfo).ToLowerInvariant());
    }
}
