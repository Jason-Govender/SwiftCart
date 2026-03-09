using SwiftCart.Domain.Enums;

namespace SwiftCart.Domain.OrderState;

public interface IOrderState
{
    OrderStatus Status { get; }
    bool CanTransitionTo(OrderStatus target);
    IReadOnlyList<OrderStatus> GetAllowedTransitions();
}
