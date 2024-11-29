using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using ProductService.Abstractions;
using ProductService.Database;
using ProductService.Entities;
using ProductService.Extensions;
using ProductService.Features.Products;
using ProductService.Models;
using ProductService.Shared;
using System.Text.Json.Serialization;

namespace ProductService.Features.Stock
{
    public class StockAdjust
    {
        public sealed class Command : ICommand<ProductModel>
        {
            [JsonIgnore]
            public Guid Id { get; set; }
            public int Quantity { get; set; }
            public string Reason { get; set; } = string.Empty;

        }
        public sealed class Handler(ApplicationContext context, IMapper mapper) : ICommandHandler<Command, ProductModel>
        {
            public async Task<Result<ProductModel>> Handle(Command request, CancellationToken cancellationToken)
            {
                var productId = new ProductId(request.Id);
                var productResult = await Result.Try(() => context.Products.FindAsync(productId), e => ProductErrors.NotFound(productId));
                if (productResult.IsFailed) return Result.Fail(productResult.Errors);

                productResult.ValueOrDefault!.AdjustStock(request.Quantity);

                context.Products.Update(productResult.ValueOrDefault);

                await context.SaveChangesAsync(cancellationToken);

                var model = mapper.Map<ProductModel>(productResult.Value);

                return Result.Ok(model);
            }
        }

        public sealed class Validator : AbstractValidator<Command>
        {
            public Validator() 
            {
                RuleFor(x => x.Quantity).NotNull().GreaterThan(0);
                RuleFor(x => x.Reason).NotEmpty();
            }
        }

        public sealed class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapPost("products/{productId:guid}/adjust-stock", async (Guid productId,Command command, ISender sender) =>
                {
                    command.Id = productId;
                    var result = await sender.Send(command);

                    return result.Match(
                        onSuccess: () => Results.Ok(result.ToApiResponse()),
                        onError: (_) => Results.Ok(result.ToApiResponse(errorCode: StatusCodes.Status404NotFound))
                        );
                }).RequireAuthorization(Constants.AdminRole);
            }
        }
    }
}
