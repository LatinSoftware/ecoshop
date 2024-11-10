using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UserService.Behaviors;
using UserService.Database;

namespace UserService.Test
{
    public class BaseTest : IDisposable
    {

        public readonly ApplicationContext context;
        public readonly ISender sender;
        public readonly IServiceProvider serviceProvider;

        public BaseTest()
        {
            var services = new ServiceCollection();

            services.AddDbContext<ApplicationContext>(options =>
            {
                options.UseInMemoryDatabase(Guid.NewGuid().ToString());
            });

            services.AddMediatR(options =>
            {
                options.RegisterServicesFromAssembly(typeof(DependencyInyection).Assembly);
                options.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            });

            services.AddValidatorsFromAssembly(typeof(DependencyInyection).Assembly);

            services.AddAutoMapper(typeof(DependencyInyection).Assembly);

            serviceProvider = services.BuildServiceProvider();

            sender = serviceProvider.GetRequiredService<ISender>();
            context = serviceProvider.GetRequiredService<ApplicationContext>();
        }

        public void Dispose()
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }
    }
}