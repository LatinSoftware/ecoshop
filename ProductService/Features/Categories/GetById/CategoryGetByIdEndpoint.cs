using MediatR;
using ProductService.Abstractions;
using ProductService.Entities;

namespace ProductService.Features.Categories.GetById
{
    public class CategoryGetByIdEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("categories/{id:guid}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new CategoryGetByIdCommand(new CategoryId(id)));
                if (result.IsSuccess)
                    return Results.Ok(result.Value);

                return Results.NotFound(result.Errors);
            });
        }
    }
}
