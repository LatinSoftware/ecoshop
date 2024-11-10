using FluentValidation;

namespace ProductService.Features.Products.Update
{
    public sealed class ProductUpdateValidator : AbstractValidator<ProductUpdateCommand>
    {
        public ProductUpdateValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty().NotNull();
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0).PrecisionScale(10, 2, true);
            RuleFor(x => x.Description).MinimumLength(50).MaximumLength(1500);
        }
    }
}
