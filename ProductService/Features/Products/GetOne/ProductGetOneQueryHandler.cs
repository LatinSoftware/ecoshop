using AutoMapper;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using ProductService.Abstractions;
using ProductService.Database;
using ProductService.Models;

namespace ProductService.Features.Products.GetOne
{
    public sealed class ProductGetOneQueryHandler(ApplicationContext context, IMapper mapper) : IQueryHandler<ProductGetOneQuery, ProductModel>
    {
        public async Task<Result<ProductModel>> Handle(ProductGetOneQuery request, CancellationToken cancellationToken)
        {
            var entity = await context.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken: cancellationToken);
            if (entity == null)
                return Result.Fail(ProductErrors.NotFound(request.ProductId));

            var model = mapper.Map<ProductModel>(entity);
            return Result.Ok(model);
        }
    }
}
