using SwiftCart.Application.Helpers;
using SwiftCart.Application.Services;
using SwiftCart.Domain.Entities;

namespace SwiftCart.Presentation.Menus;

public class CustomerMenu
{
    private readonly AuthService _authService;
    private readonly ProductService _productService;
    private readonly CartService _cartService;
    private readonly WalletService _walletService;
    private readonly OrderService _orderService;

    public CustomerMenu(AuthService authService, ProductService productService, CartService cartService, WalletService walletService, OrderService orderService)
    {
        _authService = authService;
        _productService = productService;
        _cartService = cartService;
        _walletService = walletService;
        _orderService = orderService;
    }

    public void Run(Customer user)
    {
        while (true)
        {
            Console.WriteLine($"\n--- Customer Menu (logged in as {user.Username}) ---");
            Console.WriteLine("1.  Browse Products");
            Console.WriteLine("2.  Search Products");
            Console.WriteLine("3.  Add Product to Cart");
            Console.WriteLine("4.  View Cart");
            Console.WriteLine("5.  Update Cart");
            Console.WriteLine("6.  Checkout");
            Console.WriteLine("7.  View Wallet Balance");
            Console.WriteLine("8.  Add Wallet Funds");
            Console.WriteLine("9.  View Order History");
            Console.WriteLine("10. Track Orders");
            Console.WriteLine("11. Review Products");
            Console.WriteLine("12. Logout");

            int choice = InputHelper.ReadInt("Select option: ", 1, 12);

            if (choice == 12)
            {
                _authService.Logout();
                Console.WriteLine("You have been logged out.");
                InputHelper.WaitForAnyKey("Press any key to continue.");
                return;
            }

            switch (choice)
            {
                case 1:
                    HandleBrowseProducts();
                    break;
                case 2:
                    HandleSearchProducts();
                    break;
                case 3:
                    HandleAddToCart(user);
                    break;
                case 4:
                    HandleViewCart(user);
                    break;
                case 5:
                    HandleUpdateCart(user);
                    break;
                case 6:
                    HandleCheckout(user);
                    break;
                case 7:
                    HandleViewWalletBalance(user);
                    break;
                case 8:
                    HandleAddWalletFunds(user);
                    break;
                case 9:
                    HandleViewOrderHistory(user);
                    break;
                case 10:
                    HandleTrackOrders(user);
                    break;
                default:
                    Console.WriteLine("Not implemented yet.");
                    break;
            }

            InputHelper.WaitForAnyKey("Press any key to return to menu.");
        }
    }

    private void HandleBrowseProducts()
    {
        var products = _productService.GetAll();
        if (products.Count == 0)
        {
            Console.WriteLine("No products available.");
            return;
        }
        foreach (var p in products)
            Console.WriteLine($"  [{p.Id}] {p.Name} - ${p.Price:N2} (Stock: {p.StockQuantity})");
    }

    private void HandleSearchProducts()
    {
        string term = InputHelper.ReadString("Search term: ");
        var products = _productService.Search(term);
        if (products.Count == 0)
        {
            Console.WriteLine("No matching products.");
            return;
        }
        foreach (var p in products)
            Console.WriteLine($"  [{p.Id}] {p.Name} - ${p.Price:N2} (Stock: {p.StockQuantity})");
    }

    private void HandleAddToCart(Customer user)
    {
        int productId = InputHelper.ReadInt("Product ID: ", 1, int.MaxValue);
        int quantity = InputHelper.ReadInt("Quantity: ", 1, 9999);
        if (_cartService.AddItem(user.Id, productId, quantity))
            Console.WriteLine("Product added to cart.");
        else
            Console.WriteLine("Invalid product ID or insufficient stock.");
    }

    private void HandleViewCart(Customer user)
    {
        var cart = _cartService.GetCart(user.Id);
        if (cart.Items.Count == 0)
        {
            Console.WriteLine("Your cart is empty.");
            return;
        }
        decimal total = 0;
        foreach (var item in cart.Items)
        {
            var product = _productService.GetById(item.ProductId);
            string name = product?.Name ?? $"Product #{item.ProductId}";
            decimal lineTotal = item.Quantity * item.UnitPrice;
            total += lineTotal;
            Console.WriteLine($"  [{item.Id}] {name} x {item.Quantity} @ ${item.UnitPrice:N2} = ${lineTotal:N2}");
        }
        Console.WriteLine($"  Total: ${total:N2}");
    }

    private void HandleUpdateCart(Customer user)
    {
        var cart = _cartService.GetCart(user.Id);
        if (cart.Items.Count == 0)
        {
            Console.WriteLine("Your cart is empty.");
            return;
        }
        decimal total = 0;
        foreach (var item in cart.Items)
        {
            var product = _productService.GetById(item.ProductId);
            string name = product?.Name ?? $"Product #{item.ProductId}";
            decimal lineTotal = item.Quantity * item.UnitPrice;
            total += lineTotal;
            Console.WriteLine($"  [{item.Id}] {name} x {item.Quantity} @ ${item.UnitPrice:N2} = ${lineTotal:N2}");
        }
        Console.WriteLine($"  Total: ${total:N2}");
        int cartItemId = InputHelper.ReadInt("Cart item ID to update: ", 1, int.MaxValue);
        int newQuantity = InputHelper.ReadInt("New quantity (0 to remove): ", 0, 9999);
        if (_cartService.UpdateItemQuantity(user.Id, cartItemId, newQuantity))
            Console.WriteLine(newQuantity == 0 ? "Item removed from cart." : "Cart updated.");
        else
            Console.WriteLine("Item not found in your cart.");
    }

    private void HandleViewWalletBalance(Customer user)
    {
        decimal balance = _walletService.GetBalance(user.Id);
        Console.WriteLine($"Your wallet balance: ${balance:N2}");
    }

    private void HandleAddWalletFunds(Customer user)
    {
        decimal amount = InputHelper.ReadDecimal("Amount to add: $", 0.01m, 999_999.99m);
        if (_walletService.AddFunds(user.Id, amount))
            Console.WriteLine($"${amount:N2} added to your wallet.");
        else
            Console.WriteLine("Failed to add funds.");
    }

    private void HandleCheckout(Customer user)
    {
        var (success, order, errorMessage) = _orderService.PlaceOrder(user.Id);
        if (success && order != null)
            Console.WriteLine($"Order placed successfully. Order #{order.Id} - Total: ${order.TotalAmount:N2}");
        else
            Console.WriteLine($"Checkout failed: {errorMessage ?? "Unknown error."}");
    }

    private void HandleViewOrderHistory(Customer user)
    {
        var orders = _orderService.GetOrdersByCustomer(user.Id);
        if (orders.Count == 0)
        {
            Console.WriteLine("You have no orders yet.");
            return;
        }
        foreach (var o in orders)
            Console.WriteLine($"  Order #{o.Id} - {o.CreatedAt:yyyy-MM-dd HH:mm} - ${o.TotalAmount:N2} - {o.Status}");
    }

    private void HandleTrackOrders(Customer user)
    {
        var orders = _orderService.GetOrdersByCustomer(user.Id);
        if (orders.Count == 0)
        {
            Console.WriteLine("You have no orders to track.");
            return;
        }
        foreach (var o in orders)
            Console.WriteLine($"  Order #{o.Id} - Status: {o.Status} - ${o.TotalAmount:N2} - {o.CreatedAt:yyyy-MM-dd HH:mm}");
        int orderId = InputHelper.ReadInt("Enter order ID for details (or 0 to skip): ", 0, int.MaxValue);
        if (orderId == 0)
            return;
        var order = _orderService.GetOrderById(orderId);
        if (order == null || order.CustomerId != user.Id)
        {
            Console.WriteLine("Order not found.");
            return;
        }
        Console.WriteLine($"Order #{order.Id} - Status: {order.Status} - Total: ${order.TotalAmount:N2} - Placed: {order.CreatedAt:yyyy-MM-dd HH:mm}");
        if (order.Items != null && order.Items.Count > 0)
        {
            foreach (var item in order.Items)
            {
                var product = _productService.GetById(item.ProductId);
                string name = product?.Name ?? $"Product #{item.ProductId}";
                Console.WriteLine($"    {name} x {item.Quantity} @ ${item.UnitPrice:N2}");
            }
        }
    }
}
