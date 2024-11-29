using FluentResults;
using OrderService.Errors;

namespace OrderService.Entities
{
    public record OrderId(Guid Value);
    public record ProductId(Guid Value);
    public record Money(decimal Value)
    {
        public static Result<Money> Create(decimal value)
        {
            if (value < 0) return Result.Fail(GeneralErrorMessage.MoneyNegative);
            return Result.Ok(new Money(value));
        }
    }
    public record Quantity(int Value)
    {
        public static Result<Quantity> Create(int value)
        {
            if (value <= 0) return Result.Fail(GeneralErrorMessage.QuantityPositive);
            return Result.Ok(new Quantity(value));
        }
    }
}
