using SwiftCart.Application.Helpers;
using SwiftCart.Application.Interfaces;
using SwiftCart.Application.PaymentStrategies;
using SwiftCart.Domain.Entities;
using SwiftCart.Infrastructure.Data;

namespace SwiftCart.Presentation.Menus;

public class CustomerMenu
{
    private readonly IAuthService _authService;
    private readonly IProductService _productService;
    private readonly ICartService _cartService;
    private readonly IWalletService _walletService;
    private readonly IOrderService _orderService;
    private readonly IReviewService _reviewService;
    private readonly AppDb _db;
    private readonly Action _saveAll;

    public CustomerMenu(IAuthService authService, IProductService productService, ICartService cartService, IWalletService walletService, IOrderService orderService, IReviewService reviewService, AppDb db, Action saveAll)
    {
        _authService = authService;
        _productService = productService;
        _cartService = cartService;
        _walletService = walletService;
        _orderService = orderService;
        _reviewService = reviewService;
        _db = db;
        _saveAll = saveAll;
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
            int unreadCount = _db.Notifications.Count(n => n.CustomerId == user.Id && !n.IsRead);
            if (unreadCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"12. View Notifications ({unreadCount} unread)");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("12. View Notifications");
            }
            Console.WriteLine("13. Logout");

            int choice = InputHelper.ReadInt("Select option: ", 1, 13);

            if (choice == 13)
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
                case 11:
                    HandleReviewProducts(user);
                    break;
                case 12:
                    HandleViewNotifications(user);
                    break;
                default:
                    Console.WriteLine("Not implemented yet.");
                    break;
            }

            _saveAll();
            InputHelper.WaitForAnyKey("Press any key to return to menu.");
        }
    }

    private void HandleBrowseProducts()
    {
        try
        {
            var products = _productService.GetAll();
            if (products.Count == 0)
            {
                Console.WriteLine("No products available.");
                return;
            }
            foreach (var p in products)
            {
                var avg = _reviewService.GetAverageRating(p.Id);
                var count = _reviewService.GetReviewCount(p.Id);
                string ratingInfo = count == 0 ? "No reviews yet" : $"Rating: {avg:F1} ({count} review(s))";
                Console.WriteLine($"  [{p.Id}] {p.Name} - ${p.Price:N2} (Stock: {p.StockQuantity}) - {ratingInfo}");
            }
        }
        catch (Exception)
        {
            Console.WriteLine("An unexpected error occurred. Please try again.");
        }
    }

    private void HandleSearchProducts()
    {
        try
        {
            string term = InputHelper.ReadString("Search term: ");
            var products = _productService.Search(term);
            if (products.Count == 0)
            {
                Console.WriteLine("No matching products.");
                return;
            }
            foreach (var p in products)
            {
                var avg = _reviewService.GetAverageRating(p.Id);
                var count = _reviewService.GetReviewCount(p.Id);
                string ratingInfo = count == 0 ? "No reviews yet" : $"Rating: {avg:F1} ({count} review(s))";
                Console.WriteLine($"  [{p.Id}] {p.Name} - ${p.Price:N2} (Stock: {p.StockQuantity}) - {ratingInfo}");
            }
        }
        catch (Exception)
        {
            Console.WriteLine("An unexpected error occurred. Please try again.");
        }
    }

    private void HandleAddToCart(Customer user)
    {
        try
        {
            int productId = InputHelper.ReadInt("Product ID: ", 1, int.MaxValue);
            int quantity = InputHelper.ReadInt("Quantity: ", 1, 9999);
            if (_cartService.AddItem(user.Id, productId, quantity))
                Console.WriteLine("Product added to cart.");
            else
                Console.WriteLine("Invalid product ID or insufficient stock.");
        }
        catch (Exception)
        {
            Console.WriteLine("An unexpected error occurred. Please try again.");
        }
    }

    private void HandleViewCart(Customer user)
    {
        try
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
        catch (Exception)
        {
            Console.WriteLine("An unexpected error occurred. Please try again.");
        }
    }

    private void HandleUpdateCart(Customer user)
    {
        try
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
        catch (Exception)
        {
            Console.WriteLine("An unexpected error occurred. Please try again.");
        }
    }

    private void HandleViewWalletBalance(Customer user)
    {
        try
        {
            decimal balance = _walletService.GetBalance(user.Id);
            Console.WriteLine($"Your wallet balance: ${balance:N2}");
        }
        catch (Exception)
        {
            Console.WriteLine("An unexpected error occurred. Please try again.");
        }
    }

    private void HandleAddWalletFunds(Customer user)
    {
        try
        {
            decimal amount = InputHelper.ReadDecimal("Amount to add: $", 0.01m, 999_999.99m);
            if (_walletService.AddFunds(user.Id, amount))
                Console.WriteLine($"${amount:N2} added to your wallet.");
            else
                Console.WriteLine("Failed to add funds.");
        }
        catch (Exception)
        {
            Console.WriteLine("An unexpected error occurred. Please try again.");
        }
    }

    private void HandleCheckout(Customer user)
    {
        try
        {
            Console.WriteLine("Select payment method:");
            Console.WriteLine("1. Wallet");
            Console.WriteLine("2. Cash on Delivery");
            int methodChoice = InputHelper.ReadInt("Payment method: ", 1, 2);

            IPaymentStrategy paymentStrategy = methodChoice switch
            {
                1 => new WalletPaymentStrategy(_walletService),
                2 => new CashOnDeliveryPaymentStrategy(),
                _ => new WalletPaymentStrategy(_walletService)
            };

            var (success, order, errorMessage) = _orderService.PlaceOrder(user.Id, paymentStrategy);
            if (success && order != null)
                Console.WriteLine($"Order placed successfully. Order #{order.Id} - Total: ${order.TotalAmount:N2} - Payment: {paymentStrategy.MethodName}");
            else
                Console.WriteLine($"Checkout failed: {errorMessage ?? "Unknown error."}");
        }
        catch (Exception)
        {
            Console.WriteLine("An unexpected error occurred. Please try again.");
        }
    }

    private void HandleViewOrderHistory(Customer user)
    {
        try
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
        catch (Exception)
        {
            Console.WriteLine("An unexpected error occurred. Please try again.");
        }
    }

    private void HandleTrackOrders(Customer user)
    {
        try
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
        catch (Exception)
        {
            Console.WriteLine("An unexpected error occurred. Please try again.");
        }
    }

    private void HandleReviewProducts(Customer user)
    {
        try
        {
            var products = _productService.GetAll();
            if (products.Count == 0)
            {
                Console.WriteLine("No products available to review.");
                return;
            }
            foreach (var p in products)
            {
                var avg = _reviewService.GetAverageRating(p.Id);
                var count = _reviewService.GetReviewCount(p.Id);
                string ratingInfo = count == 0 ? "No reviews yet" : $"Rating: {avg:F1} ({count} review(s))";
                Console.WriteLine($"  [{p.Id}] {p.Name} - ${p.Price:N2} - {ratingInfo}");
            }
            int productId = InputHelper.ReadInt("Enter product ID to review (0 to go back): ", 0, int.MaxValue);
            if (productId == 0)
                return;
            var product = _productService.GetById(productId);
            if (product == null)
            {
                Console.WriteLine("Product not found.");
                return;
            }
            var existing = _reviewService.GetReviewByCustomerAndProduct(user.Id, productId);
            if (existing != null)
            {
                Console.WriteLine($"You already reviewed this product (rating: {existing.Rating}).");
                if (!InputHelper.ReadYesNo("Update your review? (y/n): "))
                    return;
            }
            int rating = InputHelper.ReadInt("Rating (1-5): ", 1, 5);
            string comment = InputHelper.ReadString("Comment (optional, press Enter to skip): ");
            var (success, errorMessage) = _reviewService.SubmitOrUpdateReview(user.Id, productId, rating, comment);
            if (success)
                Console.WriteLine("Review saved successfully.");
            else
                Console.WriteLine($"Failed to save review: {errorMessage ?? "Unknown error."}");
        }
        catch (Exception)
        {
            Console.WriteLine("An unexpected error occurred. Please try again.");
        }
    }

    private void HandleViewNotifications(Customer user)
    {
        try
        {
            var notifications = _db.Notifications
                .Where(n => n.CustomerId == user.Id)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();

            if (notifications.Count == 0)
            {
                Console.WriteLine("You have no notifications.");
                return;
            }

            int unreadCount = notifications.Count(n => !n.IsRead);
            Console.WriteLine($"\n--- Notifications ({unreadCount} unread) ---");

            foreach (var n in notifications)
            {
                string readMark = n.IsRead ? "[Read]  " : "[New]   ";
                Console.WriteLine($"  {readMark} {n.CreatedAt:yyyy-MM-dd HH:mm} - {n.Message}");
                n.IsRead = true;
            }
        }
        catch (Exception)
        {
            Console.WriteLine("An unexpected error occurred. Please try again.");
        }
    }
}
