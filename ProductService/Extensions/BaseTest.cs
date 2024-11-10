using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductService.Behaviors;
using ProductService.Database;
using ProductService.Features.Categories;
using ProductService.Features.Products.Create;

namespace ProductService.Extensions
{
    public class BaseTest : IDisposable
    {
        public readonly ApplicationContext context;
        public readonly IMapper mapper;
        private ServiceProvider serviceProvider;
        public readonly ISender sender;
        public BaseTest()
        {
            var configuration = new MapperConfiguration(cfg => cfg.AddMaps(typeof(CategoryMapping).Assembly));
            mapper = configuration.CreateMapper();

            var services = new ServiceCollection();
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(typeof(Program).Assembly);
                config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            });

            services.AddDbContext<ApplicationContext>(options =>
            options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            services.AddValidatorsFromAssemblyContaining<ProductCreateCommandValidator>();

            services.AddAutoMapper(typeof(Program).Assembly);

            serviceProvider = services.BuildServiceProvider();

            sender = serviceProvider.GetRequiredService<ISender>();
            context = serviceProvider.GetRequiredService<ApplicationContext>();
        }

        public void Dispose()
        {
            ClearDatabase();
            context.Database.EnsureDeleted();
            context.Dispose();
        }

        public void ClearDatabase()
        {
            context.Categories.RemoveRange(context.Categories);
            context.SaveChanges();
        }
    }
}
