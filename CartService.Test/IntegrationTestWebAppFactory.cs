using CartService.Abstractions.Repositories;
using CartService.Abstractions.Services;
using CartService.Behaviors;
using CartService.Database.Repositories;
using CartService.Test.MockServices;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using Testcontainers.MongoDb;


namespace CartService.Test
{
    public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly MongoDbContainer dbContainer = new MongoDbBuilder().WithImage("mongo:latest").WithPassword("SuperStrongPassword").Build();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                var descriptorType = typeof(IMongoDatabase);

                var descriptor = services.SingleOrDefault(s => s.ServiceType == descriptorType);

                if (descriptor is not null)
                    services.Remove(descriptor);

                var mongoClient = new MongoClient(dbContainer.GetConnectionString());
                services.AddSingleton(provider => mongoClient.GetDatabase("ShoppingCart"));
                services.AddScoped<ICartRepository, CartRepository>();
                services.AddScoped<IProductService, ProductMockService>();
                services.AddMediatR(options =>
            {
                options.RegisterServicesFromAssembly(typeof(Program).Assembly);
                options.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            });
            
                services.AddMediatR(options =>
                {
                    options.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
                    options.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
                });

                services.AddAutoMapper(typeof(DependencyInjection).Assembly);
                services.AddValidatorsFromAssembly(typeof(Program).Assembly);

                

            });
        }

        public Task InitializeAsync()
        {
            return dbContainer.StartAsync();
        }

        Task IAsyncLifetime.DisposeAsync()
        {
           return dbContainer.StopAsync();
        }
    }
}
