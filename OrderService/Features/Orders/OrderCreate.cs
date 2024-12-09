using FluentResults;
using MassTransit;
using MassTransit.Mediator;
using OrderService.Abstractions;
using OrderService.Entities;
using OrderService.Errors;
using OrderService.Models;

namespace OrderService.Features.Orders
{
    public sealed class OrderCreate
    {
        public record Command : Request<Result<OrderCreatedModel>>
        {
            public string CartId { get; set; } = string.Empty;
            public Guid UserId { get; set; }
            public PaymentMethod PaymentMethod { get; set; }
        }

        public sealed class Handler(ICartApi cartApi, IProductApi productApi) : MediatorRequestHandler<Command, Result<OrderCreatedModel>>
        {
            protected override async Task<Result<OrderCreatedModel>> Handle(Command request, CancellationToken cancellationToken)
            {
                // get user cart
                var cartResult = await cartApi.GetByUserId(request.UserId);

                // validate if cart is not empty

                if (cartResult.Items.Count == 0)
                    return Result.Fail("Cart do not have items!!!!");

                // validate products stock and price

                var productTask = cartResult.Items.Select(GetAvailableProduct);

                var productResponses = await Task.WhenAll(productTask);
                var productResult = Result.Merge(productResponses);

                if (productResult.IsFailed)
                    return Result.Fail(productResult.Errors);

                // create payment authorization

                // create order

                return Result.Ok(new OrderCreatedModel
                {
                    OrderId = Guid.NewGuid(),
                });
            }

           
            private async Task<Result<ProductModel>> GetAvailableProduct(CartItem item)
            {
                var productResponse = await productApi.GetStock(item.ProductId);

                if(!productResponse.IsSuccessful)
                {
                    return Result.Fail($"Product with id: {item.ProductId} not found");
                }

                var product = productResponse.Content.Data!;

                var result = Result.Merge(
                    Result.FailIf(product.AvailableQuantity == 0,$"Product ${product.Name} is not available."),
                    Result.FailIf(product.Price != item.Price, $"Product ${product.Name} has an invalid price.")
                    );

               if(result.IsFailed) return result;

               return Result.Ok(product);
            }


        }

        public sealed class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapPost("/orders", async (Command command, IMediator mediator) =>
                {
                    var result = await mediator.SendRequest(command);

                    return result.Errors;
                });
            }
        }
    }
}
