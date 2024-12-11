using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Abstractions;
using ProductService.Entities;
using ProductService.Extensions;
using ProductService.Shared;

namespace ProductService.Features.Categories.Update
{
    public class CategoryUpdateEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPatch("categories/{id:guid}", async (Guid id, [FromBody]CategoryUpdateCommand command, ISender sender) =>
            {
                command.CategoryId = new CategoryId(id);
                var result = await sender.Send(command);

                return result.Match
                (
                    onSuccess: () => Results.NoContent(),
                    onError: (errors) =>
                    {
                        var categoryError = CategoryErrors.NotFound(command.CategoryId);
                        if (errors.HasErrorWithCode(categoryError.Code))
                            return Results.NotFound(result.ToApiResponse(errorCode: StatusCodes.Status404NotFound));

                        if(errors.HasErrorWithCode(CategoryErrors.AlreadyExist.Code))
                            return Results.Conflict(result.ToApiResponse(errorCode: StatusCodes.Status409Conflict));

                        return Results.BadRequest(result.ToApiResponse(errorCode: StatusCodes.Status400BadRequest));
                    }
                );
            }) ;
        }
    }
}
