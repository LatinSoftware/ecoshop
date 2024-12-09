using MassTransit;
namespace OrderService.Features.StateMachines
{
    public class OrderStateMachine : MassTransitStateMachine<OrderState>
    {
        public State Submitted { get; private set; }
        public State Accepted { get; private set; }

        public Event<SubmitOrder> SubmitOrder { get; private set; }
        public Event<OrderAccepted> OrderAccepted { get; private set; }

#pragma warning disable CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de declararlo como que admite un valor NULL.
        public OrderStateMachine()
#pragma warning restore CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de declararlo como que admite un valor NULL.
        {
            InstanceState(x => x.CurrentState, Submitted, Accepted);
            Event(() => SubmitOrder, x => x.CorrelateById(context => context.Message.OrderId));

            Initially(
                When(SubmitOrder).TransitionTo(Submitted)
                );

            Event(() => OrderAccepted, x => x.CorrelateById(context => context.Message.OrderId));
            During(Submitted, When(OrderAccepted).TransitionTo(Accepted));


        }
    }

    public class OrderState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get ; set ; }
        public int CurrentState { get; set; }
    }

    public interface SubmitOrder
    {
        Guid OrderId { get; }
    }

    public interface OrderAccepted
    {
        Guid OrderId { get; }
    }
}
