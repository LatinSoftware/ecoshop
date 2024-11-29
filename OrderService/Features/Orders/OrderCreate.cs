using OrderService.Abstractions;
using OrderService.Entities;
using OrderService.Models;

namespace OrderService.Features.Orders
{
    public sealed class OrderCreate
    {
        public sealed class Command : ICommand<OrderCreatedModel>
        {
            public Guid CartId { get; set; }
            public Guid UserId { get; set; }
            public PaymentMethod PaymentMethod { get; set; }
        }

        public sealed class Handler : ICommandHandler<Command,OrderCreatedModel>
        {

        }
    }
}
