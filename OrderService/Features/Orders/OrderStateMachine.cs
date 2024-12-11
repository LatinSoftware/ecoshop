using MassTransit;
using OrderService.Contracts;

namespace OrderService.Features.Orders;

public class OrderStateMachine : MassTransitStateMachine<OrderStateMachineData>
{

    public State Pending { get; private set; }
    public State PaymentAuthorized { get; private set; }
    public State Shipped { get; private set; }
    public State Delivered { get; private set; }
    public State Failed { get; private set; }
    public State Cancelled { get; private set; }

    public Event<OrderCreated> OrderSubmitted { get; private set; }
    public Event<PaymentAuthorized> PaymentAuthorizedEvent { get; private set; }
    public Event<PaymentAuthorizationFailed> PaymentAuthorizationFailedEvent { get; private set; }
    public Event<OrderShipped> OrderShippedEvent { get; private set; }
    public Event<ShippingFailed> ShippingFailedEvent { get; private set; }
    public Event<OrderDelivered> OrderDeliveredEvent { get; private set; }
    public Event<OrderCompleted> OrderCompletedEvent { get; private set; }
    public Event<OrderCancelled> OrderCancelledEvent { get; private set; }


    public OrderStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => OrderSubmitted, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => PaymentAuthorizedEvent, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => PaymentAuthorizationFailedEvent, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => OrderShippedEvent, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => ShippingFailedEvent, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => OrderDeliveredEvent, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => OrderCancelledEvent, x => x.CorrelateById(context => context.Message.OrderId));


        Initially(
                When(OrderSubmitted)
                    .Then(context =>
                    {
                        context.Saga.OrderId = context.Message.OrderId;
                        context.Saga.CreatedAt = DateTime.UtcNow;
                    })
                    .TransitionTo(Pending)
                    .Publish(context => new AuthorizePayment(context.Saga.OrderId, context.Message.TotalAmount)));

        During(Pending, 
            When(PaymentAuthorizedEvent)
            .Then(context => context.Saga.PaymentTransactionId = context.Message.TransactionId)
            .TransitionTo(PaymentAuthorized),
            When(PaymentAuthorizationFailedEvent).TransitionTo(Failed)
            );

        During(PaymentAuthorized,
            When(OrderShippedEvent).TransitionTo(Shipped),
            When(ShippingFailedEvent).TransitionTo(Failed)
            );

        During(Shipped,
            When(OrderDeliveredEvent)
            .Then(context => context.Saga.CompletedAt = DateTime.UtcNow)
            .TransitionTo(Delivered).Finalize()
            );

        DuringAny(When(OrderCancelledEvent).TransitionTo(Cancelled).Finalize());

    }
}

