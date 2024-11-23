using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace CartService.Test
{
    public abstract class BaseIntregationTest : IClassFixture<IntegrationTestWebAppFactory>, IDisposable
    {
        private readonly IServiceScope scope;
        protected readonly ISender Sender;
        protected readonly IMongoDatabase Database;
        protected BaseIntregationTest(IntegrationTestWebAppFactory factory)
        {
           
            scope = factory.Services.CreateScope();
            Sender = scope.ServiceProvider.GetRequiredService<ISender>();
            Database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
        }
        public void Dispose()
        {
            scope?.Dispose();
            Database?.Client?.Dispose();
        }
    }
}
