namespace OrderService.Models
{
    public sealed class PaymentConfirmModel
    {
        public record Request(string PaymentIntentId, string PaymentMethodId);
        public record Response(string Id, string Status, decimal Amount);
    }
}
