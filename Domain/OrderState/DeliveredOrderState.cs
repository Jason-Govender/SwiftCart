using SwiftCart.Domain.Enums;

namespace SwiftCart.Domain.OrderState;

public class DeliveredOrderState : IOrderState
{
    public OrderStatus Status => OrderStatus.Delivered;

    public bool CanTransitionTo(OrderStatus target) => false;

    public IReadOnlyList<OrderStatus> GetAllowedTransitions() =>
        Array.Empty<OrderStatus>();
}
