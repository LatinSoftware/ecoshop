using CartService.Abstractions;
using CartService.Abstractions.Repositories;
using CartService.Abstractions.Services;
using CartService.Entities;
using CartService.Features.CartItems;
using CartService.Models;
using FluentResults;
using FluentValidation;
using MediatR;

namespace CartService.Features.Carts
{
    public partial class CartCreate
    {
        public record Command(Guid UserId, List<CartItemRequest> Items) : ICommand<Cart>;
        public sealed class Handler(ICartRepository repository, IProductService productService) : ICommandHandler<Command, Cart>
        {
            public async Task<Result<Cart>> Handle(Command request, CancellationToken cancellationToken)
            {
                //var result = await repository.GetByUserId(request.UserId);

                //if(result.IsFailed) return result;

                var userCart = Cart.Create(request.UserId);

                request.Items.ForEach(async item =>
                {
                    var productResult = await productService.GetAsync(item.ProductId);

                    if (productResult.IsFailed) return;

                    var price = productResult.Value.Price;

                    userCart.AddItem(CartItem.Create(userCart.Id!, item.ProductId, item.Quantity, price, price * item.Quantity));
                });


                await repository.CreateAsync(userCart);

                return Result.Ok(userCart);

            }
        }

        public sealed class CartValidator : AbstractValidator<Command>
        {
            public CartValidator()
            {
                RuleFor(c => c.UserId).NotNull().NotEmpty();
                RuleForEach(c => c.Items).NotNull().SetValidator(new CartItemValidator()).When(c => c.Items.Count > 0);
            }
        }

        public sealed class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapPost("cart", async (Command command, ISender sender) =>
                {
                    var result = await sender.Send(command);

                    if (result.IsFailed)
                        return Results.BadRequest(result.Value);

                    return Results.CreatedAtRoute("userCart", new { userId = command.UserId }, result.Value);
                });
            }
        }
    }
}
