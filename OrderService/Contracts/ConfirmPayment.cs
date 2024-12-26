namespace OrderService.Contracts
{
    public record ConfirmPayment(Guid OrderId, string PaymentId, decimal Amount);
}
