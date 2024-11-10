using MediatR;
using ProductService.Abstractions;
using ProductService.Entities;

namespace ProductService.Features.Products.Update
{
    public class ProductUpdateEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("products/{id:guid}", async (Guid id, ProductUpdateCommand command, ISender sender) =>
            {
                command.ProductId = id;
                var result = await sender.Send(command);

                return result switch
                {
                    { IsSuccess: true } => Results.NoContent(),
                    { IsFailed: true, Errors: var errors } when errors.Contains(ProductErrors.NotFound(ProductId.Create(command.ProductId))) => Results.NotFound(result),
                    _ => Results.BadRequest(result)
                };
            });
        }
    }
}
