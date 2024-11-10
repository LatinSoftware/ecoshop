using AutoMapper;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using ProductService.Abstractions;
using ProductService.Database;
using ProductService.Entities;
using ProductService.Features.Categories;
using ProductService.Models;

namespace ProductService.Features.Products.Create
{
    public class ProductCreateCommandHandler(ApplicationContext context, IMapper mapper) : ICommandHandler<ProductCreateCommand, ProductModel>
    {
        public async Task<Result<ProductModel>> Handle(ProductCreateCommand request, CancellationToken cancellationToken)
        {

            var categoryId = new CategoryId(request.CategoryId);
            var categoryExist = await context.Categories.AnyAsync(c => c.Id == categoryId, cancellationToken: cancellationToken);

            if (!categoryExist)
                return Result.Fail(CategoryErrors.NotFound(categoryId));

            var entity = Product.Create(categoryId, request.Name, request.Price, request.Description);
            await context.Products.AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            var model = mapper.Map<ProductModel>(entity);
            return Result.Ok(model);
        }
    }
}
