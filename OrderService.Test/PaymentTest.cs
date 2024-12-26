using OrderService.Services;
using Stripe;

namespace OrderService.Test;

public class PaymentTest
{
    [Fact]
    public async Task PaymentService_should_be_ok()
    {
        StripeConfiguration.ApiKey = "";

        var service = new PaymentService();
        var payment =  await service.Intent(new Models.PaymentIntentModel.Request(8.1m, "usd", "PaymentService_should_be_ok"));

        await service.Confirm(payment.Value.Id);
    }
}
