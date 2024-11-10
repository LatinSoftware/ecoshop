using MediatR;
using ProductService.Abstractions;
using ProductService.Entities;

namespace ProductService.Features.Products.GetOne
{
    public sealed class ProductGetOneEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("products/{id:guid}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new ProductGetOneQuery(ProductId.Create(id)));

                return result switch
                {
                    { IsSuccess: true} => Results.Ok(result.Value),
                    _ => Results.NotFound(result.Errors)
                };

            });
        }
    }
}
