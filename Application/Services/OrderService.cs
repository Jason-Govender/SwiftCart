using SwiftCart.Application.Interfaces;
using SwiftCart.Domain.Entities;
using SwiftCart.Domain.Enums;
using SwiftCart.Infrastructure.Data;

namespace SwiftCart.Application.Services;

public class OrderService : IOrderService
{
    private readonly AppDb _db;
    private readonly ICartService _cartService;
    private readonly IProductService _productService;
    private readonly List<IOrderObserver> _observers = new();

    public OrderService(AppDb db, ICartService cartService, IProductService productService)
    {
        _db = db;
        _cartService = cartService;
        _productService = productService;
    }

    public void Subscribe(IOrderObserver observer)
    {
        if (!_observers.Contains(observer))
            _observers.Add(observer);
    }

    public void Unsubscribe(IOrderObserver observer)
    {
        _observers.Remove(observer);
    }

    /// <summary>
    /// Places an order from the customer's cart using the supplied payment strategy.
    /// Returns (true, order, null) on success, or (false, null, errorMessage) on failure.
    /// </summary>
    public (bool Success, Order? Order, string? ErrorMessage) PlaceOrder(int customerId, IPaymentStrategy paymentStrategy)
    {
        try
        {
            var cart = _cartService.GetCart(customerId);
            if (cart.Items == null || cart.Items.Count == 0)
                return (false, null, "Your cart is empty.");

            decimal total = 0;
            foreach (var item in cart.Items)
            {
                var product = _productService.GetById(item.ProductId);
                if (product == null)
                    return (false, null, $"Product #{item.ProductId} is no longer available.");
                if (product.StockQuantity < item.Quantity)
                    return (false, null, $"Insufficient stock for {product.Name}. Available: {product.StockQuantity}, requested: {item.Quantity}.");
                total += item.Quantity * item.UnitPrice;
            }

            var (paySuccess, payError) = paymentStrategy.Pay(customerId, total);
            if (!paySuccess)
                return (false, null, payError ?? "Payment failed.");

            int orderId = GetNextOrderId();
            var order = new Order
            {
                Id = orderId,
                CustomerId = customerId,
                TotalAmount = total,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                Items = new List<OrderItem>()
            };

            int orderItemId = GetNextOrderItemId();
            foreach (var item in cart.Items)
            {
                order.Items.Add(new OrderItem
                {
                    Id = orderItemId++,
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                });
                if (!_productService.DeductStock(item.ProductId, item.Quantity))
                {
                    paymentStrategy.Refund(customerId, total);
                    return (false, null, "Failed to reserve stock. Please try again.");
                }
            }

            _db.Orders.Add(order);

            _db.Payments.Add(new Payment
            {
                Id = GetNextPaymentId(),
                OrderId = order.Id,
                CustomerId = customerId,
                Amount = total,
                Method = paymentStrategy.MethodName,
                PaidAt = DateTime.UtcNow
            });

            cart.Items.Clear();
            NotifyOrderPlaced(order);
            return (true, order, null);
        }
        catch (Exception)
        {
            return (false, null, "An unexpected error occurred while placing your order.");
        }
    }

    public List<Order> GetOrdersByCustomer(int customerId)
    {
        return _db.Orders
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedAt)
            .ToList();
    }

    public Order? GetOrderById(int orderId)
    {
        return _db.Orders.FirstOrDefault(o => o.Id == orderId);
    }

    public List<Order> GetAllOrders()
    {
        return _db.Orders.OrderByDescending(o => o.CreatedAt).ToList();
    }

    public bool UpdateOrderStatus(int orderId, OrderStatus status)
    {
        var order = _db.Orders.FirstOrDefault(o => o.Id == orderId);
        if (order == null)
            return false;
        var previousStatus = order.Status;
        order.Status = status;
        NotifyOrderStatusChanged(order, previousStatus);
        return true;
    }

    private void NotifyOrderPlaced(Order order)
    {
        foreach (var observer in _observers)
            observer.OnOrderPlaced(order);
    }

    private void NotifyOrderStatusChanged(Order order, OrderStatus previousStatus)
    {
        foreach (var observer in _observers)
            observer.OnOrderStatusChanged(order, previousStatus);
    }

    private int GetNextOrderId()
    {
        if (_db.Orders == null || _db.Orders.Count == 0)
            return 1;
        return _db.Orders.Max(o => o.Id) + 1;
    }

    private int GetNextPaymentId()
    {
        if (_db.Payments == null || _db.Payments.Count == 0)
            return 1;
        return _db.Payments.Max(p => p.Id) + 1;
    }

    private int GetNextOrderItemId()
    {
        if (_db.Orders == null || _db.Orders.Count == 0)
            return 1;
        int max = _db.Orders
            .Where(o => o.Items != null)
            .SelectMany(o => o.Items)
            .Select(i => i.Id)
            .DefaultIfEmpty(0)
            .Max();
        return max + 1;
    }
}
