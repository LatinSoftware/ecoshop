using MediatR;
using ProductService.Abstractions;
using ProductService.Entities;
using ProductService.Extensions;

namespace ProductService.Features.Categories.GetById
{
    public class CategoryGetByIdEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("categories/{id:guid}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new CategoryGetByIdCommand(new CategoryId(id)));

                return result.Match
                (
                    onSuccess: () => Results.Ok(result.ToApiResponse(200)),
                    onError: (_) => Results.NotFound(result.ToApiResponse(errorCode: 404))
                );
            });
        }
    }
}
