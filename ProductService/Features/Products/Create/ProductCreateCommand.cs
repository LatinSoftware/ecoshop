using ProductService.Abstractions;
using ProductService.Models;

namespace ProductService.Features.Products.Create
{
    public class ProductCreateCommand : ICommand<ProductModel>
    {
        public string Name { get;  set; } = string.Empty;
        public decimal Price { get;  set; }
        public string? Description { get;  set; }
        public Guid CategoryId { get;  set; }
    }
}
