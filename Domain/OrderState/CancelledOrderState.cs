using SwiftCart.Domain.Enums;

namespace SwiftCart.Domain.OrderState;

public class CancelledOrderState : IOrderState
{
    public OrderStatus Status => OrderStatus.Cancelled;

    public bool CanTransitionTo(OrderStatus target) => false;

    public IReadOnlyList<OrderStatus> GetAllowedTransitions() =>
        Array.Empty<OrderStatus>();
}
