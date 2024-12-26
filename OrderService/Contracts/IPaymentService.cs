using FluentResults;
using OrderService.Models;

namespace OrderService.Contracts;

public interface IPaymentService
{
    Task<Result<PaymentConfirmModel.Response>> Confirm(string paymentIntentId);
    Task<Result<PaymentIntentModel.Response>> Intent(PaymentIntentModel.Request request);
}
