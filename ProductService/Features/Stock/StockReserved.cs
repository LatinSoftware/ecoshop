using FluentResults;
using FluentValidation;
using MediatR;
using ProductService.Abstractions;
using ProductService.Database;
using ProductService.Entities;
using ProductService.Extensions;
using ProductService.Features.Products;
using System.Text.Json.Serialization;

namespace ProductService.Features.Stock
{
    public class StockReserved
    {
        public sealed class Command : ICommand
        {
            [JsonIgnore]
            public Guid ProductId { get; set; }
            public int Quantity { get; set;}
        }

        public sealed class Handler(ApplicationContext context) : ICommandHandler<Command>
        {
            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var productId = new ProductId(request.ProductId);
                var productResult = await Result.Try(() => context.Products.FindAsync(productId), e => ProductErrors.NotFound(productId));
                if (productResult.IsFailed) return Result.Fail(productResult.Errors);

                var product = productResult.ValueOrDefault!;

                var result = product.ReserveStock(request.Quantity);

                if (result.IsFailed) return Result.Fail(result.Errors);

                context.Products.Update(product);

                await context.SaveChangesAsync(cancellationToken);

                return Result.Ok().WithSuccess($"Product {product.Name} reserved correctly");
            }
        }

        public sealed class Validator : AbstractValidator<Command>
        {
            public Validator() 
            {
                RuleFor(x => x.Quantity).NotNull().GreaterThan(0);
            }
        }

        public sealed class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapPost("products/{productId:guid}/reserve-stock", async (Guid productId, Command command, ISender sender) =>
                {
                    command.ProductId = productId;
                    var result = await sender.Send(command);

                    return result.Match
                    (
                        onSuccess: () => Results.NoContent(),
                        onError: (errors) =>
                        {
                            if (errors.HasErrorWithCode(ProductErrors.NotFoundContent.Code))
                                return Results.NotFound(result.ToApiResponse(errorCode: StatusCodes.Status404NotFound));

                            return Results.Conflict(result.ToApiResponse(errorCode: StatusCodes.Status409Conflict));
                        }
                    );
                });
            }
        }
    }
}
