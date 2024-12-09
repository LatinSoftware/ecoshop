namespace OrderService.Errors
{
    public static class OrderErrorMessage
    {
        public static DomainError ItemNotFound { get; } = new DomainError("Item.NotFound", "Item not found");
        public static DomainError OrderIdNull { get; } = new DomainError("Order.InvalidID", "OrderId cannot be null.");
        public static DomainError TransactionIdNull { get; } = new DomainError("Order.InvalidTransactionId", "TransactionId cannot be empty.");
        public static DomainError OrderAlreadyPaid { get; } = new DomainError("Order.OrderAlreadyPaid", "Payment is already marked as paid.");
    }
}
