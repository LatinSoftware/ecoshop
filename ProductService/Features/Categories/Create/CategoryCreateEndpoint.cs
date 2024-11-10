using MediatR;
using ProductService.Abstractions;

namespace ProductService.Features.Categories.Create
{
    public class CategoryCreateEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("categories", async (CategoryCreateCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);

                if (result.IsFailed)
                    return Results.BadRequest(result.Errors);

                return Results.Created($"categories/{result.Value.Id}",result.Value);
            });
        }
    }
}
