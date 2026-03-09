using SwiftCart.Domain.Entities;
using SwiftCart.Domain.Enums;

namespace SwiftCart.Application.Interfaces;

public interface IOrderObserver
{
    void OnOrderPlaced(Order order);
    void OnOrderStatusChanged(Order order, OrderStatus previousStatus);
}
