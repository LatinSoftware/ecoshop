using CartService.Abstractions;
using CartService.Abstractions.Repositories;
using CartService.Abstractions.Services;
using CartService.Entities;
using CartService.Models;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CartService.Features.CartItems
{
    public class CartAddItem
    {
        public record Command(string CartId, CartItemRequest Item) : ICommand;
        public sealed class Handler(ICartRepository cartRepository, IProductService productService) : ICommandHandler<Command>
        {
            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var result = await cartRepository.GetAsync(request.CartId);
                if (result.IsFailed)
                    return Result.Fail(result.Errors);

                var cart = result.Value;

               

                if (!cart.Items.Any(x => x.ProductId == request.Item.ProductId))
                {
                    var product = await productService.GetAsync(request.Item.ProductId);
                    if (product.IsFailed) return Result.Fail(product.Errors);

                    var item = request.Item;
                    var price = product.Value.Price;
                    var cartItem = CartItem.Create(cart.Id!, item.ProductId, item.Quantity, price, item.Quantity * price);
                    cart.AddItem(cartItem);
                   
                }
                else
                {
                    var itemToUpdate = cart.Items.First(x => x.ProductId == request.Item.ProductId);
                    itemToUpdate.SetQuantity(itemToUpdate.Quantity + request.Item.Quantity);
                    
                }


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
                app.MapPost("cart/{id}/items", async (string id, [FromBody]CartItemRequest items, ISender sender) =>
                {
                    var result = await sender.Send(new Command(id, items));
                    if (result.IsFailed) return Results.NotFound(result.Errors);
                    return Results.Created();
                });
            }
        }
    }
}
