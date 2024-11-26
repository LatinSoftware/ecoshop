using CartService.Abstractions.Repositories;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace CartService.Test
{
    public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>, IDisposable
    {
        private readonly IServiceScope scope;
        protected readonly ISender Sender;
        protected readonly IMongoDatabase Database;
        protected readonly ICartRepository CartRepository;
        protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
        {
           
            scope = factory.Services.CreateScope();
            Sender = scope.ServiceProvider.GetRequiredService<ISender>();
            Database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
            CartRepository = scope.ServiceProvider.GetRequiredService<ICartRepository>();
        }
        public void Dispose()
        {
            scope?.Dispose();
           
        }
    }
}
