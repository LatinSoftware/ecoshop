namespace OrderService.Errors
{
    public static class GeneralErrorMessage
    {
        public static DomainError MoneyNegative { get; } = new DomainError("Money.Negative", "Money cannot be negative.");
        public static DomainError QuantityPositive { get; } = new DomainError("Quantity.MustPositive", "Quantity must be greater than zero.");
    }
}
