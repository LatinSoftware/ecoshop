using CartService.Abstractions;
using CartService.Abstractions.Repositories;
using CartService.Entities;
using CartService.Models;
using FluentResults;
using FluentValidation;
using MediatR;

namespace CartService.Features.CartItems
{
    public class CartAddItem
    {
        public record Command(string CartId, CartItemRequest Item) : ICommand;
        public sealed class Handler(ICartRepository cartRepository) : ICommandHandler<Command>
        {
            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var result = await cartRepository.GetAsync(request.CartId);
                if (result.IsFailed)
                    return Result.Fail(result.Errors);

                var cart = result.Value;

                var item = request.Item;
                var cartItem = CartItem.Create(cart.Id!, item.ProductId, item.Quantity, item.Price, item.Quantity * item.Price);
                cart.AddItem(cartItem);
                await cartRepository.UpdateAsync(request.CartId, cart);

                return Result.Ok();

            }
        }

        public sealed class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.CartId).NotEmpty().NotNull();
                RuleFor(x => x.Item).NotNull().SetValidator(new CartItemValidator());
            }
        }

        public sealed class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapPost("cart/{id:string}/items", async (string id, CartItemRequest items, ISender sender) =>
                {
                    var result = await sender.Send(new Command(id, items));
                    if (result.IsFailed) return Results.NotFound(result.Errors);
                    return Results.Created();
                });
            }
        }
    }
}
