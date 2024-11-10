using FluentResults;
using Microsoft.EntityFrameworkCore;
using ProductService.Abstractions;
using ProductService.Database;
using ProductService.Entities;

namespace ProductService.Features.Products.Update
{
    public sealed class ProductUpdateCommandHandler(ApplicationContext context) : ICommandHandler<ProductUpdateCommand>
    {
        public async Task<Result> Handle(ProductUpdateCommand request, CancellationToken cancellationToken)
        {
            var productId = ProductId.Create(request.ProductId);
            var product = await context.Products.FirstOrDefaultAsync(x => x.Id == productId, cancellationToken: cancellationToken);

            if (product == null)
            {
                return Result.Fail(ProductErrors.NotFound(productId));
            }

            product.SetName(request.Name);
            product.SetPrice(request.Price);
            product.SetDescription(request.Description);

            context.Products.Update(product);
            await context.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}
