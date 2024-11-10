using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductService.Database;

namespace ProductService.Features.Categories.Update
{
    public class CategoryUpdateCommandHandler(ApplicationContext context) : IRequestHandler<CategoryUpdateCommand, Result>
    {
        public async Task<Result> Handle(CategoryUpdateCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Name))
                return Result.Fail(CategoryErrors.InvalidName);


            var entity = await context.Categories.FirstOrDefaultAsync(x => x.Id == request.CategoryId, cancellationToken: cancellationToken);
            if (entity == null)
                return Result.Fail(CategoryErrors.NotFound(request.CategoryId));


            if(await context.Categories.AnyAsync(c => c.Name.ToLower() == request.Name.ToLower(), cancellationToken: cancellationToken))
                return Result.Fail(CategoryErrors.AlreadyExist);

            entity.SetName(request.Name);

            context.Categories.Update(entity);

            await context.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }

}
