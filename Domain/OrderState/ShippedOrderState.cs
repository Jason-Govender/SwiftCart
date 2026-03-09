using SwiftCart.Domain.Enums;

namespace SwiftCart.Domain.OrderState;

public class ShippedOrderState : IOrderState
{
    public OrderStatus Status => OrderStatus.Shipped;

    public bool CanTransitionTo(OrderStatus target) =>
        target == OrderStatus.Delivered;

    public IReadOnlyList<OrderStatus> GetAllowedTransitions() =>
        new[] { OrderStatus.Delivered };
}
