using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Abstractions;

namespace ProductService.Features.Products.Create
{
    public class ProductCreateEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("products", async ([FromBody]ProductCreateCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Ok(result);
            });
        }
    }
}
