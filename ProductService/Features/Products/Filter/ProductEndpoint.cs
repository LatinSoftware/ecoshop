using MediatR;
using ProductService.Abstractions;
using ProductService.Extensions;

namespace ProductService.Features.Products.Filter
{
    public sealed class ProductEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("products", async ([AsParameters]ProductFilterQuery query, ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result.ToApiResponse());
            });
        }
    }
}
