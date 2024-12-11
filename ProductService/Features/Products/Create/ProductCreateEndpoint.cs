using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Abstractions;
using ProductService.Extensions;
using ProductService.Shared;

namespace ProductService.Features.Products.Create
{
    public class ProductCreateEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("products", async ([FromBody]ProductCreateCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);

            return result.Match(
                onSuccess: () => Results.CreatedAtRoute("ProductById", new { id = result.Value.Id },
                    result.ToApiResponse(StatusCodes.Status201Created)),
                onError: (_) => Results.NotFound(result.ToApiResponse(errorCode: StatusCodes.Status404NotFound)));
            }); 
        }
    }
}
