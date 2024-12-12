using FluentResults;


namespace OrderService.Entities
{
    public class OrderItem
    {
        private OrderItem()
        {

        }
        public Guid Id { get; private set; }
        public OrderId OrderId { get; private set; } = new OrderId(Guid.Empty);
        public ProductId ProductId { get; private set; } = new ProductId(Guid.Empty);
        public Quantity Quantity { get; private set; } = new Quantity(0);
        public Money Price { get; private set; } = new Money(0);
        public Money Total => Money.Create(Quantity.Value * Price.Value).ValueOrDefault;
        public Order? Order { get; private set; }

        public static Result<OrderItem> Create(OrderId orderId, ProductId productId, Quantity quantity, Money price)
        {
            var validationResult = Result.Merge(
                Result.FailIf(orderId == null || orderId.Value == Guid.Empty, "OrderId cannot be null."),
                Result.FailIf(productId == null || productId.Value == Guid.Empty, "ProductId cannot be null."),
                Result.FailIf(quantity == null || quantity.Value <= 0, "Quantity must be greater than zero."),
                Result.FailIf(price == null || price.Value < 0, "Price must be non-negative.")
                );

            if (validationResult.IsFailed) return validationResult;

            return Result.Ok(new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = orderId!,
                ProductId = productId!,
                Quantity = quantity!,
                Price = price!
            });
        }
    }
}
