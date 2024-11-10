using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductService.Database;
using ProductService.Models;

namespace ProductService.Features.Categories.GetAll
{
    public class CategoryGetAllCommandHandler(ApplicationContext context, IMapper mapper) : IRequestHandler<CategoryGetAllCommand, Result<ICollection<CategoryModel>>>
    {
        public async Task<Result<ICollection<CategoryModel>>> Handle(CategoryGetAllCommand request, CancellationToken cancellationToken)
        {
            var entities = await context.Categories.ToListAsync(cancellationToken);
            var models = mapper.Map<ICollection<CategoryModel>>(entities);
            return Result.Ok(models);
        }
    }
}
