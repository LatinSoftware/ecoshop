using MassTransit;
using OrderService.Contracts;
using OrderService.Database;
using OrderService.Entities;

namespace OrderService.Features.Payments
{
    public class PaymentCreate
    {
        public sealed class AuthorizePaymentConsumer(
            IPaymentService paymentService,
            ApplicationContext applicationContext,
            ILogger<AuthorizePaymentConsumer> logger
            
            ) : IConsumer<ConfirmPayment>
        {
            public async Task Consume(ConsumeContext<ConfirmPayment> context)
            {
                string message = $"Se ha realizado una confirmacion pago de la orden: '${context.Message.OrderId}' por el monto de {context.Message.Amount:n2}";
                logger.LogInformation(message);

                var paymentResult = await paymentService.Confirm(context.Message.PaymentId);
                if (paymentResult.IsFailed) return;

                var paymentEntity = Payment.Create(new OrderId(context.Message.OrderId), PaymentMethod.CART, paymentResult.Value.Id);

                await applicationContext.AddAsync( paymentEntity );

                await applicationContext.SaveChangesAsync(context.CancellationToken);

                await context.Publish(new PaymentConfirmed(context.Message.OrderId, paymentEntity.Value.PaymentId, paymentResult.Value.Id, paymentResult.Value.Amount));
            }
        }
    }
}
