using AutoMapper;
using CartService.Abstractions;
using CartService.Abstractions.Repositories;
using CartService.Models;
using FluentResults;
using MediatR;

namespace CartService.Features.Carts
{
    public partial class CartByUser
    {
        public record Query(Guid UserId) : IQuery<CartModel>;
        public sealed class Handler(ICartRepository repository, IMapper mapper) : IQueryHandler<Query, CartModel>
        {
            public async Task<Result<CartModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                var cart = await repository.GetByUserId(request.UserId);
                if (cart.IsFailed)
                    return Result.Fail(cart.Errors);

                var model = mapper.Map<CartModel>(cart.Value);

                return model;

            }
        }

        public sealed class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapGet("cart/{userId:guid}", async (Guid userId, ISender sender) =>
                {
                    var result = await sender.Send(new Query(userId));

                    if (result.IsFailed) return Results.NotFound(result.Errors);

                    return Results.Ok(result.Value);
                })
                    .WithName("userCart")
                .WithGroupName("Cart")
                .RequireAuthorization();
                ;
            }
        }
    }
}
