using SwiftCart.Application.Interfaces;
using SwiftCart.Domain.Entities;
using SwiftCart.Domain.Enums;
using SwiftCart.Infrastructure.Data;

namespace SwiftCart.Application.Observers;

public class OrderNotificationObserver : IOrderObserver
{
    private readonly AppDb _db;

    public OrderNotificationObserver(AppDb db)
    {
        _db = db;
    }

    public void OnOrderPlaced(Order order)
    {
        AddNotification(order.CustomerId,
            $"Your order #{order.Id} has been placed successfully. Total: ${order.TotalAmount:N2}. Status: {order.Status}.");
    }

    public void OnOrderStatusChanged(Order order, OrderStatus previousStatus)
    {
        AddNotification(order.CustomerId,
            $"Order #{order.Id} status updated from {previousStatus} to {order.Status}.");
    }

    private void AddNotification(int customerId, string message)
    {
        int nextId = _db.Notifications.Count > 0 ? _db.Notifications.Max(n => n.Id) + 1 : 1;
        _db.Notifications.Add(new Notification
        {
            Id = nextId,
            CustomerId = customerId,
            Message = message,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        });
    }
}
