using FluentValidation;

namespace ProductService.Features.Products.Create
{
    public class ProductCreateCommandValidator : AbstractValidator<ProductCreateCommand>
    {
        public ProductCreateCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().NotNull().MaximumLength(500);
            RuleFor(x => x.Price).GreaterThan(0).PrecisionScale(10, 2, true);
            RuleFor(x => x.Description).MinimumLength(50).MaximumLength(1500);
            RuleFor(x => x.CategoryId).NotEmpty();
        }
    }
}
