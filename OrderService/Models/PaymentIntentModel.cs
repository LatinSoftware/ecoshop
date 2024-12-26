namespace OrderService.Models
{
    public sealed class PaymentIntentModel
    {
        public record Request(decimal Amount, string Currency, string? Description = "");
        public record Response (string Id, string ClientSecret);
    }
}
