using OrderService.Services;
using Stripe;

namespace OrderService.Test;

public class PaymentTest
{
    [Fact]
    public async Task PaymentService_should_be_ok()
    {
        StripeConfiguration.ApiKey = "sk_test_51QWdyaGGs74eAK9sFFE5ApmUsiZy3sGk2zNChyXKIRW5a2dfahCD6fkxPCWzMMX0EsoJN4aVqehPd6X98clMx29600zJOlMFLf";

        var service = new PaymentService();
        var payment =  await service.Intent(new Models.PaymentIntentModel.Request(8.1m, "usd", "PaymentService_should_be_ok"));

        await service.Confirm(payment.Value.Id);
    }
}
