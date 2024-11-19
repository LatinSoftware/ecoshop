using CartService.Abstractions;
using CartService.Abstractions.Repositories;
using CartService.Errors;
using FluentResults;
using FluentValidation;
using MediatR;

namespace CartService.Features.CartItems
{
    public class CartItemUpdateQuantity
    {
        public record Command(string CartId, string ItemId, int Quantity) : ICommand;
        public sealed class Handler(ICartRepository repository) : ICommandHandler<Command>
        {
            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var cart = await repository.GetAsync(request.CartId);
                if (cart.IsFailed) return Result.Fail(CartErrorMessage.NotFound(request.CartId));

                var item = cart.Value.Items.FirstOrDefault(i => i.Id == request.ItemId);
                if (item == null)
                    return Result.Fail(ItemErrorMessage.NotFound(request.ItemId));

                item.SetQuantity(request.Quantity);

                await repository.UpdateAsync(cart.Value.Id!, cart.Value);
                return Result.Ok();
            }
        }
        public sealed class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.CartId).NotEmpty().NotNull();
                RuleFor(x => x.ItemId).NotNull().NotEmpty();
                RuleFor(x => x.Quantity).NotNull().NotEmpty().GreaterThan(0);
            }
        }

        public sealed class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapPatch("cart/{cartId}/items/{itemId}/{quantity}", async (
                    string cartId,
                    string itemId,
                    int quantity,
                    ISender sender
                    ) =>
                {

                    var result = await sender.Send(new Command(cartId, itemId, quantity));

                    if (result.IsFailed) return Results.NotFound(result.Errors);

                    return Results.NoContent();

                });
            }
        }
    }
}
