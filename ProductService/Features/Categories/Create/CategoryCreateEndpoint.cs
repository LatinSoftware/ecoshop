using MediatR;
using ProductService.Abstractions;
using ProductService.Extensions;
using ProductService.Shared;

namespace ProductService.Features.Categories.Create
{
    public class CategoryCreateEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("categories", async (CategoryCreateCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);

                return result.Match(
                    onSuccess: () => Results.Created($"categories/{result.Value.Id}", result.ToApiResponse(StatusCodes.Status201Created)),
                    onError: (error) => Results.BadRequest(result.ToApiResponse(errorCode: StatusCodes.Status404NotFound)));
            })
                .WithName(Constants.Category.CreateEndpointName)
                .WithTags(Constants.Category.Tag)
                ;

        }
    }
}
