using MassTransit;
using OrderService.Contracts;

namespace OrderService.Features.Payments
{
    public class PaymentCreate
    {
        public sealed class Consumer(ILogger logger) : IConsumer<AuthorizePayment>
        {
            public async Task Consume(ConsumeContext<AuthorizePayment> context)
            {
                string message = $"Se ha realizado el pago de la orden: '${context.Message.OrderId}' por el monto de {context.Message.Amount:n2}";
                logger.LogInformation(message);

                await context.Publish(new PaymentAuthorized(context.Message.OrderId, Guid.NewGuid()));
            }
        }
    }
}
