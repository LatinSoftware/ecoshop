namespace OrderService.Contracts
{
    public record AuthorizePayment(Guid OrderId, decimal Amount);
}
