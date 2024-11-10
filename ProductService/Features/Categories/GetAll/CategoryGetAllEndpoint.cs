using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Abstractions;

namespace ProductService.Features.Categories.GetAll
{
    public class CategoryGetAllEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("categories", async ( [FromServices]ISender sender) =>
            {
                var result = await sender.Send(new CategoryGetAllCommand());
                return Results.Ok(result.Value);
            }).WithTags("Categories");
        }
    }
}
