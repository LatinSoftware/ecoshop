using MediatR;
using ProductService.Abstractions;
using ProductService.Extensions;
using ProductService.Shared;

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

               var response = result.Match(
                    onSuccess: () => Results.NoContent(),
                    onError: (_) => Results.NotFound(result.ToApiResponse(errorCode: StatusCodes.Status404NotFound, message: ProductErrors.NotFoundContent.Message))
                    
                    );

                return response;
            }).RequireAuthorization(Constants.AdminRole); ;
        }
    }
}
