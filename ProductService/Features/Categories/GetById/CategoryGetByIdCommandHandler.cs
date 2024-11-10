using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductService.Database;
using ProductService.Models;

namespace ProductService.Features.Categories.GetById
{
    internal class CategoryGetByIdCommandHandler(ApplicationContext context, IMapper mapper) : IRequestHandler<CategoryGetByIdCommand, Result<CategoryModel>>
    {
        public async Task<Result<CategoryModel>> Handle(CategoryGetByIdCommand request, CancellationToken cancellationToken)
        {
            var entity = await context.Categories.FirstOrDefaultAsync(c => c.Id.Equals(request.Id), cancellationToken);
            if (entity == null)
                return Result.Fail(CategoryErrors.NotFound(request.Id));

            var model = mapper.Map<CategoryModel>(entity);
            return Result.Ok(model);
        }
    }
}