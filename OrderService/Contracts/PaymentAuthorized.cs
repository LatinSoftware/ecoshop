namespace OrderService.Contracts;

public record PaymentAuthorized(Guid OrderId, Guid TransactionId);
