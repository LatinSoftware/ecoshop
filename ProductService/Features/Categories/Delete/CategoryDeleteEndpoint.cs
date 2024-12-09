using MediatR;
using ProductService.Abstractions;
using ProductService.Entities;
using ProductService.Shared;

namespace ProductService.Features.Categories.Delete
{
    public class CategoryDeleteEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("categories/{id:guid}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new CategoryDeleteCommand(new CategoryId(id)));

                if (result.IsSuccess)
                    return Results.NoContent();

                return Results.NotFound(result.Errors);
                
            }).RequireAuthorization(Constants.AdminRole); ;
        }
    }
}
