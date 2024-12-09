using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductService.Abstractions;
using ProductService.Database;
using ProductService.Entities;
using ProductService.Extensions;
using ProductService.Features.Products;
using ProductService.Models;

namespace ProductService.Features.Stock
{
    public class StockAvailability
    {
        public record Query(Guid ProductId) : IQuery<ProductStockModel>;


        public sealed class Handler(ApplicationContext context, IMapper mapper) : IQueryHandler<Query, ProductStockModel>
        {
            public async Task<Result<ProductStockModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                var productId = new ProductId(request.ProductId);
                
                var product = await context.Products.FirstOrDefaultAsync(x => x.Id == productId);

                if (product == null)
                    return Result.Fail(ProductErrors.NotFound(productId));

                var model = mapper.Map<ProductStockModel>(product);
                return Result.Ok(model);
              
            }
        }

        public sealed class Mapping : Profile
        {
            public Mapping() 
            {
                CreateMap<Product, ProductStockModel>()
                    .ForMember(dest => dest.AvailableQuantity, opt => opt.MapFrom(src => src.StockQuantity - src.ReservedQuantity));
            }
        }


        public sealed class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapGet("products/{productId:guid}/availability", async (Guid productId,ISender sender) =>
                {
                    
                    var result = await sender.Send(new Query(productId));

                    return result.Match
                    (
                        onSuccess: () => Results.Ok(result.ToApiResponse()),
                        onError: (_) => Results.NotFound(result.ToApiResponse(errorCode: StatusCodes.Status404NotFound))
                    );
                });
            }
        }
    }
}
