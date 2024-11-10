
using FluentResults;
using Microsoft.EntityFrameworkCore;
using ProductService.Abstractions;
using ProductService.Database;

namespace ProductService.Features.Products.Delete
{
    public sealed class ProductDeleteCommandHandler(ApplicationContext context) : ICommandHandler<ProductDeleteCommand>
    {
        public async Task<Result> Handle(ProductDeleteCommand request, CancellationToken cancellationToken)
        {
            var entity = await context.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken: cancellationToken);
            if (entity == null)
                return Result.Fail(ProductErrors.NotFound(request.ProductId));

            context.Products.Remove(entity);
            await context.SaveChangesAsync(cancellationToken);
            return Result.Ok();
        }
    }
}
