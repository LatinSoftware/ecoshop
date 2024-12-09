using MediatR;
using ProductService.Abstractions;
using ProductService.Entities;
using ProductService.Extensions;

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
                    { IsSuccess: true} => Results.Ok(result.ToApiResponse()),
                    _ => Results.NotFound(result.ToApiResponse(errorCode: StatusCodes.Status404NotFound))
                };

            }).WithName("ProductById");
        }
    }
}
