using AutoMapper;
using CartService.Abstractions;
using CartService.Abstractions.Repositories;
using CartService.Models;
using FluentResults;
using MediatR;

namespace CartService.Features.Carts
{
    public class CartGetAll
    {
        public sealed class Query : IQuery<ICollection<CartModel>> { }
        public sealed class Handler(ICartRepository repository, IMapper mapper) : IQueryHandler<Query, ICollection<CartModel>>
        {
            public async Task<Result<ICollection<CartModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var carts = await repository.GetAsync(x => true);

                var model = mapper.Map<ICollection<CartModel>>(carts);
                return Result.Ok(model);
            }
        }

        public sealed class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapGet("carts", async (ISender sender) =>
                {

                    var result = await sender.Send(new Query());

                    return Results.Ok(result.Value);

                }).WithGroupName("Cart");
            }
        }
    }
}
