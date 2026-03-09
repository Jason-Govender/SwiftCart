using SwiftCart.Domain.Enums;

namespace SwiftCart.Domain.OrderState;

public class ConfirmedOrderState : IOrderState
{
    public OrderStatus Status => OrderStatus.Confirmed;

    public bool CanTransitionTo(OrderStatus target) =>
        target == OrderStatus.Shipped || target == OrderStatus.Cancelled;

    public IReadOnlyList<OrderStatus> GetAllowedTransitions() =>
        new[] { OrderStatus.Shipped, OrderStatus.Cancelled };
}
