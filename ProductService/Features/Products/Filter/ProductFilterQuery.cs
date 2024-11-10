using ProductService.Abstractions;
using ProductService.Models;

namespace ProductService.Features.Products.Filter
{
    public sealed class ProductFilterQuery : IQuery<ICollection<ProductModel>>
    {
        public string? Name { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set;}
        public string? Description { get; set; }
        public Guid? CategoryId { get; set; }
    }
}
