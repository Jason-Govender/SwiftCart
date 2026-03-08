using SwiftCart.Domain.Entities;
using SwiftCart.Domain.Enums;
using SwiftCart.Infrastructure.Data;

namespace SwiftCart.Application.Services;

public class OrderService
{
    private readonly AppDb _db;
    private readonly CartService _cartService;
    private readonly WalletService _walletService;
    private readonly ProductService _productService;

    public OrderService(AppDb db, CartService cartService, WalletService walletService, ProductService productService)
    {
        _db = db;
        _cartService = cartService;
        _walletService = walletService;
        _productService = productService;
    }

    /// <summary>
    /// Places an order from the customer's cart. Returns (true, order, null) on success,
    /// or (false, null, errorMessage) on failure.
    /// </summary>
    public (bool Success, Order? Order, string? ErrorMessage) PlaceOrder(int customerId)
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

        if (_walletService.GetBalance(customerId) < total)
            return (false, null, "Insufficient wallet balance.");

        if (!_walletService.DeductFunds(customerId, total))
            return (false, null, "Failed to deduct funds from wallet.");

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
                _walletService.AddFunds(customerId, total);
                return (false, null, "Failed to reserve stock. Please try again.");
            }
        }

        _db.Orders.Add(order);
        cart.Items.Clear();
        return (true, order, null);
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
        order.Status = status;
        return true;
    }

    private int GetNextOrderId()
    {
        if (_db.Orders == null || _db.Orders.Count == 0)
            return 1;
        return _db.Orders.Max(o => o.Id) + 1;
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
