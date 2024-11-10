using Microsoft.EntityFrameworkCore;
using ProductService.Entities;
using ProductService.Extensions;

namespace ProductService.Features.Categories
{
    public class CategoryFixture : BaseTest
    {
        public async Task<List<Category>> GetCategories()
        {
            var categories = new List<Category>
                {
                    Category.Create("Electrónica"),
                    Category.Create("Moda"),
                    Category.Create("Hogar y Cocina"),
                    Category.Create("Deportes y Aire Libre"),
                    Category.Create("Juguetes y Juegos"),
                    Category.Create("Belleza y Cuidado Personal"),
                    Category.Create("Libros"),
                };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
            return categories;
        }

        public async Task<Category> GetCategory()
        {
            var category = Category.Create("Naturaleza");

            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();
            return category;
        }

        public async Task<Category?> GetCategory(CategoryId categoryId)
        {
           return await context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);  
        }
    }
}
