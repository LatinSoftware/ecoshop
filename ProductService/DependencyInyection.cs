using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ProductService.Abstractions;
using ProductService.Behaviors;
using ProductService.Features.Products.Create;
using System.Reflection;

namespace ProductService
{
    public static class DependencyInyection
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<Database.ApplicationContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("Default"));
            });

            services.AddMediatR(options => {
                options.RegisterServicesFromAssembly(typeof(Program).Assembly);
                options.AddBehavior(typeof(IPipelineBehavior<,>) ,typeof(ValidationBehavior<,>));
            });
            services.AddAutoMapper(typeof(Program).Assembly);
            services.AddValidatorsFromAssemblyContaining<ProductCreateCommandValidator>();



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

            var logger = app.Logger;

            foreach (var endpoint in builder.DataSources.SelectMany(ds => ds.Endpoints))
            {

                logger.LogInformation($"Endpoint encontrado: {endpoint.DisplayName}");
            }


            return app;
        }
    }
}
