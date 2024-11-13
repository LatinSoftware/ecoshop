using CartService.Abstractions;
using CartService.Abstractions.Repositories;
using CartService.Entities;
using CartService.Models;
using FluentResults;
using FluentValidation;
using MediatR;

namespace CartService.Features
{
    public class CartCreate
    {
        public record Command(Guid UserId, List<CartItemRequest> Items) : ICommand<Cart>;
        public sealed class Handler(ICartRepository repository) : ICommandHandler<Command, Cart>
        {
            public async Task<Result<Cart>> Handle(Command request, CancellationToken cancellationToken)
            {
                //var result = await repository.GetByUserId(request.UserId);

                //if(result.IsFailed) return result;

                var userCart = Cart.Create(request.UserId);

                request.Items.ForEach(item => userCart.AddItem(
                    CartItem.Create(userCart.Id!, item.ProductId, item.Quantity, item.Price, item.Price * item.Quantity)));


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

        public sealed class CartItemValidator : AbstractValidator<CartItemRequest>
        {
            public CartItemValidator()
            {
                RuleFor(c => c.ProductId).NotNull().NotEmpty();
                RuleFor(c => c.Quantity).NotNull().GreaterThan(0);
                RuleFor(c => c.Price).NotNull().GreaterThanOrEqualTo(0).PrecisionScale(10,2, false);
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

                    return Results.CreatedAtRoute("userCart", new {userId = command.UserId}, result.Value);
                });
            }
        }
    }
}
