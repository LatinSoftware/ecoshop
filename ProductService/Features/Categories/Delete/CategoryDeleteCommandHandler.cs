using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductService.Database;

namespace ProductService.Features.Categories.Delete
{
    public class CategoryDeleteCommandHandler(ApplicationContext context) : IRequestHandler<CategoryDeleteCommand, Result>
    {
        public async Task<Result> Handle(CategoryDeleteCommand request, CancellationToken cancellationToken)
        {
            var entity = await context.Categories.FirstOrDefaultAsync(x => x.Id == request.CategoryId, cancellationToken: cancellationToken);
            if (entity == null)
                return Result.Fail(CategoryErrors.NotFound(request.CategoryId));
            
            context.Categories.Remove(entity);

            await context.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}
