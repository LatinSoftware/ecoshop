using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using OrderService.Abstractions;
using OrderService.Database;
using OrderService.Features;
using OrderService.Features.Orders;
using Refit;
using System.Reflection;

namespace OrderService
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {

            var client = new MongoClient(configuration.GetConnectionString("Default"));
            ApplicationContext.Create(client.GetDatabase("orders"));


            services.AddMediator(x => x.AddConsumersFromNamespaceContaining<Consumers>());

            services.AddMassTransit(x =>
            {
                x.AddSagaStateMachine<OrderStateMachine, OrderStateMachineData>().InMemoryRepository();
            });

            services.AddRefitClient<ICartApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri("https://localhost:8081/api/v1"));
            services.AddRefitClient<IProductApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri("https://localhost:8082/api/v1"));
            
            return services;
        }

        public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
        {
            ServiceDescriptor[] serviceDescriptors = assembly
                .DefinedTypes
                .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                               type.IsAssignableTo(typeof(IEndpoint)))
                .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
                .ToArray();

            services.TryAddEnumerable(serviceDescriptors);

            return services;
        }

        public static IApplicationBuilder MapEndpoints(this WebApplication app, RouteGroupBuilder? routeGroupBuilder = null)
        {
            IEnumerable<IEndpoint> endpoints = app.Services
                .GetRequiredService<IEnumerable<IEndpoint>>();

            IEndpointRouteBuilder builder =
                routeGroupBuilder is null ? app : routeGroupBuilder;

            foreach (IEndpoint endpoint in endpoints)
            {
                endpoint.MapEndpoint(builder);
            }

            return app;
        }
    }
}
