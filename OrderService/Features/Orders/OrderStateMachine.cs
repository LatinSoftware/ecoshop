using MassTransit;
using OrderService.Contracts;
using OrderService.Entities;

namespace OrderService.Features.Orders;

public class OrderStateMachine : MassTransitStateMachine<OrderStateMachineData>
{

    public State Pending { get; private set; }
    public State PaymentConfirmed { get; private set; }
    public State Shipped { get; private set; }
    public State Delivered { get; private set; }
    public State Failed { get; private set; }
    public State Cancelled { get; private set; }

    public Event<OrderCreated> OrderCreated { get; private set; }
    public Event<PaymentConfirmed> PaymentConfirmedEvent { get; private set; }
    public Event<PaymentAuthorizationFailed> PaymentConfirmationFailedEvent { get; private set; }
    public Event<OrderShipped> OrderShippedEvent { get; private set; }
    public Event<ShippingFailed> ShippingFailedEvent { get; private set; }
    public Event<OrderDelivered> OrderDeliveredEvent { get; private set; }
    public Event<OrderCancelled> OrderCancelledEvent { get; private set; }


    public OrderStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => OrderCreated, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => PaymentConfirmedEvent, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => PaymentConfirmationFailedEvent, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => OrderShippedEvent, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => ShippingFailedEvent, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => OrderDeliveredEvent, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => OrderCancelledEvent, x => x.CorrelateById(context => context.Message.OrderId));


        Initially(
                When(OrderCreated)
                    .Then(context =>
                    {
                        context.Saga.OrderId = context.Message.OrderId;
                        context.Saga.PaymentIntentId = context.Message.PaymentIntentId;
                        context.Saga.CreatedAt = DateTime.UtcNow;
                    })
                    .TransitionTo(Pending)
                    .Publish(context => new ConfirmPayment(context.Saga.OrderId, context.Message.PaymentIntentId, context.Message.TotalAmount)));

        During(Pending, 
            When(PaymentConfirmedEvent)
            .TransitionTo(PaymentConfirmed),
            When(PaymentConfirmationFailedEvent).TransitionTo(Failed)
            );

        During(PaymentConfirmed,
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

