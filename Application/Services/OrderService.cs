using SwiftCart.Application.Interfaces;
using SwiftCart.Domain.Entities;
using SwiftCart.Domain.Enums;

namespace SwiftCart.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepo;
    private readonly IPaymentRepository _paymentRepo;
    private readonly ICartService _cartService;
    private readonly IProductService _productService;
    private readonly List<IOrderObserver> _observers = new();

    public OrderService(IOrderRepository orderRepo, IPaymentRepository paymentRepo, ICartService cartService, IProductService productService)
    {
        _orderRepo = orderRepo;
        _paymentRepo = paymentRepo;
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

            var order = new Order
            {
                Id = _orderRepo.GetNextId(),
                CustomerId = customerId,
                TotalAmount = total,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                Items = new List<OrderItem>()
            };

            int orderItemId = _orderRepo.GetNextItemId();
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

            _orderRepo.Add(order);

            _paymentRepo.Add(new Payment
            {
                Id = _paymentRepo.GetNextId(),
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

    public List<Order> GetOrdersByCustomer(int customerId) =>
        _orderRepo.GetByCustomer(customerId);

    public Order? GetOrderById(int orderId) =>
        _orderRepo.GetById(orderId);

    public List<Order> GetAllOrders() =>
        _orderRepo.GetAll();

    public bool UpdateOrderStatus(int orderId, OrderStatus status)
    {
        var order = _orderRepo.GetById(orderId);
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
}
