using CartService.Abstractions;
using CartService.Abstractions.Repositories;
using CartService.Abstractions.Services;
using CartService.Entities;
using CartService.Errors;
using CartService.Features.CartItems;
using CartService.Models;
using FluentResults;
using FluentValidation;
using MediatR;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace CartService.Features.Carts
{
    public partial class CartCreate
    {
        public record Command(Guid UserId, List<CartItemRequest> Items) : ICommand<Cart>;
        public sealed class Handler(ICartRepository repository, IProductService productService) : ICommandHandler<Command, Cart>
        {
            public async Task<Result<Cart>> Handle(Command request, CancellationToken cancellationToken)
            {
                var result = await repository.GetByUserId(request.UserId);

                if(result.IsSuccess){
                    var cart = result.Value;
                    cart = AddItems(cart, request.Items);
                    await repository.UpdateAsync(cart.Id!, cart);
                    return Result.Ok(cart);
                }

                var userCart = AddItems(Cart.Create(request.UserId), request.Items);
                await repository.CreateAsync(userCart);
                return Result.Ok(userCart);

            }

            private Cart AddItems(Cart cart, List<CartItemRequest> items){
                items.ForEach(async item =>
                {
                    var productResult = await productService.GetAsync(item.ProductId);

                    if (productResult.IsFailed) return;

                    var price = productResult.Value.Price;

                    cart.AddItem(CartItem.Create(cart.Id!, item.ProductId, item.Quantity, price, price * item.Quantity));
                });

                return cart;
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
                app.MapPost("carts", async (Command command, ISender sender, ClaimsPrincipal user) =>
                {

                    var userId = Guid.Parse(user.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

                    if (command.UserId != userId)
                        return Results.BadRequest(new Result().WithError(CartErrorMessage.UserMisMatch));

                    var result = await sender.Send(command);

                    if (result.IsFailed)
                        return Results.BadRequest(result.Value);

                    return Results.CreatedAtRoute("userCart", new { userId = command.UserId }, result.Value);
                }).RequireAuthorization();
            }
        }
    }
}
