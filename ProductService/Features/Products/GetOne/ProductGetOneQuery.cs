using ProductService.Abstractions;
using ProductService.Entities;
using ProductService.Models;

namespace ProductService.Features.Products.GetOne
{
    public record ProductGetOneQuery(ProductId ProductId) : IQuery<ProductModel>;
}
