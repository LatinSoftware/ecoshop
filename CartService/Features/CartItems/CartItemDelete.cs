using CartService.Abstractions;
using CartService.Abstractions.Repositories;
using CartService.Errors;
using FluentResults;
using MediatR;

namespace CartService.Features.CartItems
{
    public class CartItemDelete
    {
        public record Command(string CartId, string ItemId) : ICommand;
        public sealed class Handler(ICartRepository repository) : ICommandHandler<Command>
        {
            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var cartResult = await repository.GetAsync(request.CartId);
                if (cartResult.IsFailed) return Result.Fail(CartErrorMessage.NotFound(request.CartId));

                var cart = cartResult.Value;

                var item = cart.Items.FirstOrDefault(i => i.Id == request.ItemId);
                if (item == null)
                    return Result.Fail(ItemErrorMessage.NotFound(request.ItemId));

                cart.DeleteItem(item);

                await repository.UpdateAsync(cart.Id!, cart);

                return Result.Ok();
            }
        }

        public sealed class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapDelete("carts/{cartId}/items/{itemId}", async (string cartId, string itemId, ISender sender) =>
                {
                    var result = await sender.Send(new Command(cartId, itemId));
                    if (result.IsFailed) return Results.NotFound(result.Errors);
                    return Results.NoContent();
                }); 
            }
        }
    }
}
