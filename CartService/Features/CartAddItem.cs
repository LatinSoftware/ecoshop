using CartService.Abstractions;
using CartService.Abstractions.Repositories;
using CartService.Models;
using FluentResults;

namespace CartService.Features
{
    public class CartAddItem
    {
        public record Command(string CartId, List<CartItemRequest> Items) : ICommand;
        public sealed class Handler(ICartRepository cartRepository) : ICommandHandler<Command>
        {
            public Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
