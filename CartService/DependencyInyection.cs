using CartService.Abstractions;
using CartService.Abstractions.Repositories;
using CartService.Behaviors;
using CartService.Database.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Reflection;
using CartService.Abstractions.Services;
using CartService.Services;

namespace CartService
{
    public static class DependencyInyection
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {

            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            var mongoClient = new MongoClient(configuration.GetConnectionString("CartDatabase"));
            var mongoDatabase = mongoClient.GetDatabase(configuration["DatabaseName"]);
            services.AddSingleton(provider => mongoDatabase);

            services.AddMediatR(options =>
            {
                options.RegisterServicesFromAssembly(typeof(Program).Assembly);
                options.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            });

            services.AddAutoMapper(typeof(Program).Assembly);
            services.AddValidatorsFromAssembly(typeof(Program).Assembly);

            services.AddHttpClient<IProductService, ProductService>("product", client =>
            {
                client.DefaultRequestHeaders.Add("User-Agent", "CartService");
                client.BaseAddress = new Uri("https://localhost:8082/api/v1/products/");
            });
            

            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<IProductService, ProductService>();


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
