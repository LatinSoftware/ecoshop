using Bogus;
using Microsoft.EntityFrameworkCore;
using ProductService.Database;
using ProductService.Entities;
using ProductService.Extensions;

namespace ProductService.Features.Products
{
    public class ProductFixture : BaseTest
    {
        public ProductFixture()
        {
            DatabaseSeed.Seed(context);
            CreateProducts().GetAwaiter().GetResult();
        }
        public async Task<Product?> GetProductById(ProductId id) => await context.Products.FirstOrDefaultAsync(x => x.Id == id);
        public async Task<Product?> GetProduct() => await context.Products.FirstOrDefaultAsync();
        public async Task<List<Product>> GetProducts() => await context.Products.ToListAsync();
        public async Task<List<Category>> GetCategories() => await context.Categories.ToListAsync();

        private async Task CreateProducts()
        {
            var categories = await GetCategories();
            var categoryIds = categories.Select(c => c.Id).ToList();

            var productRandom = new Faker<Product>()
                .CustomInstantiator(f => Product.Create(
                    f.PickRandom(categoryIds),
                    f.Commerce.ProductName(),
                    f.Commerce.Random.Decimal(1, 200),
                    f.Commerce.ProductDescription()
                ));

            var products = productRandom.Generate(50);

            context.AddRange(products);
            context.Add(Product.Create(categories.First().Id, "product #1", 10, string.Empty));
            context.Add(Product.Create(categories.First().Id, "product #2", 15, "Descripcion para el producto #2"));
            context.Add(Product.Create(categories.First().Id, "product #3", 40, "Descripcion para el producto #3"));
            context.Add(Product.Create(categories.First().Id, "product #4", 25, "Descripcion para el producto #4"));
            await context.SaveChangesAsync();
        }

    }
}
