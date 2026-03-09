using SwiftCart.Domain.Enums;

namespace SwiftCart.Domain.OrderState;

public class PendingOrderState : IOrderState
{
    public OrderStatus Status => OrderStatus.Pending;

    public bool CanTransitionTo(OrderStatus target) =>
        target == OrderStatus.Confirmed || target == OrderStatus.Cancelled;

    public IReadOnlyList<OrderStatus> GetAllowedTransitions() =>
        new[] { OrderStatus.Confirmed, OrderStatus.Cancelled };
}
