using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Abstractions;
using ProductService.Entities;
using ProductService.Exceptions;

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

                if (result.IsSuccess)
                    return Results.NoContent();

                if (result.HasError<ApplicationError>(error => error.Code == CategoryErrors.NotFound(command.CategoryId).Code))
                {
                    return Results.NotFound(result.Errors);
                }

                return Results.BadRequest(result.Errors);

            });
        }
    }
}
