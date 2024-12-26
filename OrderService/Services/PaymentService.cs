using System;
using FluentResults;
using OrderService.Contracts;
using OrderService.Models;
using Stripe;

namespace OrderService.Services;

public class PaymentService : IPaymentService
{
    
    public async Task<Result<PaymentIntentModel.Response>> Intent(PaymentIntentModel.Request request)
    {
        var paymentOptions = new PaymentIntentCreateOptions
        {
            Amount = Convert.ToInt64(request.Amount * 100),
            Currency = request.Currency,
            Description = request.Description,
            PaymentMethod = "pm_card_visa",
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true,
                AllowRedirects = "never"
            }
        };
        var service = new PaymentIntentService();
        var payment = await service.CreateAsync(paymentOptions);

        return Result.Ok(new PaymentIntentModel.Response(payment.Id, payment.ClientSecret));
    }

    public async Task<Result<PaymentConfirmModel.Response>> Confirm(string paymentIntentId)
    {
        
        
        var service = new PaymentIntentService();
        var response = await  service.ConfirmAsync(paymentIntentId);

        return Result.Ok(new PaymentConfirmModel.Response(response.Id, response.Status, response.Amount));
    }
}
