using SwiftCart.Domain.Entities;
using SwiftCart.Domain.Enums;

namespace SwiftCart.Application.Interfaces;

public interface IOrderService
{
    (bool Success, Order? Order, string? ErrorMessage) PlaceOrder(int customerId);
    List<Order> GetOrdersByCustomer(int customerId);
    Order? GetOrderById(int orderId);
    List<Order> GetAllOrders();
    bool UpdateOrderStatus(int orderId, OrderStatus status);
}
