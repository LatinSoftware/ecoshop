using ProductService.Abstractions;

namespace ProductService.Features.Products.Update
{
    public sealed class ProductUpdateCommand : ICommand
    {
        public Guid ProductId { get; set; }
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public string? Description { get; set; }
    }
}
