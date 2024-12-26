namespace OrderService.Contracts;

public record PaymentConfirmed(Guid OrderId, Guid PaymentId, string PaymentConfirmId, decimal Amount);
