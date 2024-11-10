using ProductService.Abstractions;
using ProductService.Entities;

namespace ProductService.Features.Products.Delete
{
    public record ProductDeleteCommand(ProductId ProductId) : ICommand;
}
