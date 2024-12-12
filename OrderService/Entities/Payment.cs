using FluentResults;
using OrderService.Errors;

namespace OrderService.Entities
{
    public class Payment
    {

        public Guid PaymentId { get; set; }
        public OrderId OrderId { get; set; } = new OrderId(Guid.Empty);
        public PaymentStatus Status { get; set; }
        public PaymentMethod Method { get; set; }
        public string? TransactionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual Order? Order { get; set; }

        private Payment()
        {

        }
        private Payment(OrderId orderId, PaymentMethod method, string transactionId)
        {
            PaymentId = Guid.NewGuid();
            OrderId = orderId;
            Method = method;
            TransactionId = transactionId;
        }

        public static Result<Payment> Create(OrderId orderId, PaymentMethod method, string transactionId)
        {
            var result = Result.Merge(
                    Result.FailIf(orderId == null || orderId.Value == Guid.Empty, OrderErrorMessage.OrderIdNull),
                    Result.FailIf(string.IsNullOrWhiteSpace(transactionId), OrderErrorMessage.TransactionIdNull)
                );

            if (result.IsFailed) return result;

            return Result.Ok(new Payment(orderId!, method, transactionId));
        }

        public Result MarkAsPaid()
        {
            if (Status == PaymentStatus.PAID) return Result.Fail(OrderErrorMessage.OrderAlreadyPaid);
            Status = PaymentStatus.PAID;
            return Result.Ok();
        }
    }

    public enum PaymentStatus
    {
        UNPAID,
        PAID
    }

    public enum PaymentMethod
    {
        CART,
        PAYPAL
    }
}
