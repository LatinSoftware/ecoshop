using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductService.Database;
using ProductService.Entities;
using ProductService.Exceptions;
using ProductService.Models;

namespace ProductService.Features.Categories.Create
{
    public class CategoryCreateCommandHandler(ApplicationContext context, IMapper mapper) : IRequestHandler<CategoryCreateCommand, Result<CategoryModel>>
    {
        public async Task<Result<CategoryModel>> Handle(CategoryCreateCommand request, CancellationToken cancellationToken)
        {
            Result result = new();
            if (string.IsNullOrEmpty(request.Name))
                result.WithError(CategoryErrors.InvalidName);

            if(await context.Categories.AnyAsync(c => c.Name.ToLower().Contains(request.Name.ToLower()), cancellationToken: cancellationToken))
                result.WithError(CategoryErrors.AlreadyExist);

            if(result.HasError<ApplicationError>())
                return result;

            var entityToCreate = Category.Create(request.Name);
            await context.Categories.AddAsync(entityToCreate, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            var model = mapper.Map<CategoryModel>(entityToCreate);
            return Result.Ok(model);
        }
    }
}
