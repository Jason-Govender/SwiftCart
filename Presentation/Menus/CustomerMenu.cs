using SwiftCart.Application.Helpers;
using SwiftCart.Application.Services;
using SwiftCart.Domain.Entities;

namespace SwiftCart.Presentation.Menus;

public class CustomerMenu
{
    private readonly AuthService _authService;
    private readonly ProductService _productService;
    private readonly CartService _cartService;

    public CustomerMenu(AuthService authService, ProductService productService, CartService cartService)
    {
        _authService = authService;
        _productService = productService;
        _cartService = cartService;
    }

    public void Run(Customer user)
    {
        while (true)
        {
            Console.WriteLine($"\n--- Customer Menu (logged in as {user.Username}) ---");
            Console.WriteLine("1. Browse Products");
            Console.WriteLine("2. Search Products");
            Console.WriteLine("3. Add Product to Cart");
            Console.WriteLine("4. View Cart");
            Console.WriteLine("5. Update Cart");
            Console.WriteLine("6. Checkout");
            Console.WriteLine("7. View Wallet Balance");
            Console.WriteLine("8. Add Wallet Funds");
            Console.WriteLine("9. View Order History");
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
}
