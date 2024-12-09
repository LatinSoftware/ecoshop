using ProductService.Entities;
using ProductService.Exceptions;

namespace ProductService.Features.Products
{
    public class ProductErrors
    {
        public static ApplicationError NotFound(ProductId productId) => new("Product.NotFound", $"Product with the id '{productId.Value}' was not found");
        public static ApplicationError NotFoundContent { get; } = new("Product.NotFound", $"Product was not found");
        public static ApplicationError PriceShouldBePositive => new("Product.Positive", "Product Price should be positive");
    }
}
