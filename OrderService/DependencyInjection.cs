using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

using OrderService.Abstractions;
using OrderService.Contracts;
using OrderService.Database;
using OrderService.Entities;
using OrderService.Features;
using OrderService.Features.Orders;
using OrderService.Services;
using Refit;
using Stripe;
using System.Reflection;

namespace OrderService
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("default"));
                options.EnableDetailedErrors();
            }, contextLifetime: ServiceLifetime.Singleton);

            services.AddMediator(x => x.AddConsumersFromNamespaceContaining<Consumers>());

            services.AddMassTransit(busConfiguration =>
            {
                busConfiguration.SetKebabCaseEndpointNameFormatter();

                busConfiguration.AddConsumers(Assembly.GetExecutingAssembly());

                busConfiguration.AddSagaStateMachine<OrderStateMachine, OrderStateMachineData>().EntityFrameworkRepository(config =>
                {
                    config.UsePostgres();
                    config.ExistingDbContext<ApplicationContext>();
                });

                busConfiguration.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration.GetConnectionString("RabbitMq")!, "/", hst =>
                    {
                        hst.Username("guest");
                        hst.Password("guest");
                    });

                    cfg.UseInMemoryOutbox(context);

                    cfg.ConfigureEndpoints(context);
                });

                StripeConfiguration.ApiKey = "sk_test_51QWdyaGGs74eAK9sFFE5ApmUsiZy3sGk2zNChyXKIRW5a2dfahCD6fkxPCWzMMX0EsoJN4aVqehPd6X98clMx29600zJOlMFLf";
            });

            services.AddRefitClient<ICartApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri("https://localhost:8081/api/v1"));
            services.AddRefitClient<IProductApi>().ConfigureHttpClient(c => c.BaseAddress = new Uri("https://localhost:8082/api/v1"));
            services.AddSingleton<IPaymentService, PaymentService>();

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
