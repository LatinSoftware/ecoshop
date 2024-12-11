using MediatR;
using ProductService.Abstractions;
using ProductService.Entities;
using ProductService.Extensions;
using ProductService.Shared;

namespace ProductService.Features.Products.Delete
{
    public sealed class ProductDeleteEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("products/{id:guid}", async (Guid id, ISender sender) =>
            {
                var productId = ProductId.Create(id);
                var result = await sender.Send(new ProductDeleteCommand(productId));

                return result switch
                {
                    { IsFailed: true } => Results.NotFound(result.ToApiResponse(errorCode: StatusCodes.Status404NotFound, message: "Could not delete!")),
                    _ => Results.NoContent()
                };
            }); 
        }
    }
}
