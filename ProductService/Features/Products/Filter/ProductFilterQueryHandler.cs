using AutoMapper;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using ProductService.Abstractions;
using ProductService.Database;
using ProductService.Entities;
using ProductService.Models;

namespace ProductService.Features.Products.Filter
{
    public sealed class ProductFilterQueryHandler(ApplicationContext context, IMapper mapper) : IQueryHandler<ProductFilterQuery, ICollection<ProductModel>>
    {
        public async Task<Result<ICollection<ProductModel>>> Handle(ProductFilterQuery request, CancellationToken cancellationToken)
        {
            var query = context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(request.Name))
                query = query.Where(x => x.Name.ToLower().Contains(request.Name.ToLower()));

            if (request.MinPrice.HasValue)
                query = query.Where(p => p.Price >= request.MinPrice.Value );

            if (request.MaxPrice.HasValue)
                query = query.Where(p =>  p.Price <= request.MaxPrice.Value);

            if (!string.IsNullOrEmpty(request.Description))
                query = query.Where(p => p.Description.ToLower().Contains(request.Description.ToLower()));

            if(request.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == new CategoryId(request.CategoryId.Value));

            var data = await query.AsNoTracking().Include(p => p.Category).ToListAsync(cancellationToken: cancellationToken);
            
            var model = mapper.Map<ICollection<ProductModel>>(data);
            return Result.Ok(model);
        }
    }
}
