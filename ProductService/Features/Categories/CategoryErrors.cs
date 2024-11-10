using ProductService.Entities;
using ProductService.Extensions;

namespace ProductService.Features.Categories
{
    public static class CategoryErrors
    {
        public static ApplicationError NotFound(CategoryId categoryId) => new("Category.NotFound", $"Category with the id '{categoryId.Value}' was not found");
        public readonly static ApplicationError InvalidName = new("Category.InvalidName", "Category 'name' cannot be null or empty");
        public readonly static ApplicationError AlreadyExist = new("Category.AlreadyExist", "Category with the 'name' already exist ");
    }
}
