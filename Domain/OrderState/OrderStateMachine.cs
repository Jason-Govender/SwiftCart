using SwiftCart.Domain.Enums;

namespace SwiftCart.Domain.OrderState;

public class OrderStateMachine
{
    private readonly Dictionary<OrderStatus, IOrderState> _states = new()
    {
        [OrderStatus.Pending] = new PendingOrderState(),
        [OrderStatus.Confirmed] = new ConfirmedOrderState(),
        [OrderStatus.Shipped] = new ShippedOrderState(),
        [OrderStatus.Delivered] = new DeliveredOrderState(),
        [OrderStatus.Cancelled] = new CancelledOrderState()
    };

    public IOrderState GetState(OrderStatus status) => _states[status];
}
