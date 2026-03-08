using SwiftCart.Application.Helpers;
using SwiftCart.Application.Services;
using SwiftCart.Domain.Entities;
using SwiftCart.Domain.Enums;

namespace SwiftCart.Presentation.Menus;

public class AdministratorMenu
{
    private const int LowStockThresholdDefault = 5;
    private readonly AuthService _authService;
    private readonly ProductService _productService;
    private readonly OrderService _orderService;
    private readonly ReviewService _reviewService;

    public AdministratorMenu(AuthService authService, ProductService productService, OrderService orderService, ReviewService reviewService)
    {
        _authService = authService;
        _productService = productService;
        _orderService = orderService;
        _reviewService = reviewService;
    }

    public void Run(Administrator user)
    {
        while (true)
        {
            Console.WriteLine($"\n--- Administrator Menu (logged in as {user.Username}) ---");
            Console.WriteLine("1.  Add Product");
            Console.WriteLine("2.  Update Product");
            Console.WriteLine("3.  Delete Product");
            Console.WriteLine("4.  Restock Product");
            Console.WriteLine("5.  View Products");
            Console.WriteLine("6.  View Orders");
            Console.WriteLine("7.  Update Order Status");
            Console.WriteLine("8.  View Low Stock Products");
            Console.WriteLine("9.  Generate Sales Reports");
            Console.WriteLine("10. Logout");

            int choice = InputHelper.ReadInt("Select option: ", 1, 10);

            if (choice == 10)
            {
                _authService.Logout();
                Console.WriteLine("You have been logged out.");
                InputHelper.WaitForAnyKey("Press any key to continue.");
                return;
            }

            switch (choice)
            {
                case 1:
                    HandleAddProduct();
                    break;
                case 2:
                    HandleUpdateProduct();
                    break;
                case 3:
                    HandleDeleteProduct();
                    break;
                case 4:
                    HandleRestockProduct();
                    break;
                case 5:
                    HandleViewProducts();
                    break;
                case 6:
                    HandleViewOrders();
                    break;
                case 7:
                    HandleUpdateOrderStatus();
                    break;
                case 8:
                    HandleViewLowStock();
                    break;
                default:
                    Console.WriteLine("Not implemented yet.");
                    break;
            }

            InputHelper.WaitForAnyKey("Press any key to return to menu.");
        }
    }

    private void HandleAddProduct()
    {
        string name = InputHelper.ReadString("Product name: ");
        string description = InputHelper.ReadString("Description: ");
        decimal price = InputHelper.ReadDecimal("Price: ", 0, 999999.99m);
        int stock = InputHelper.ReadInt("Initial stock: ", 0, 100000);

        if (_productService.Add(name, description, price, stock))
            Console.WriteLine("Product added successfully.");
        else
            Console.WriteLine("Failed to add product. Name cannot be empty.");
    }

    private void HandleUpdateProduct()
    {
        var products = _productService.GetAll();
        if (products.Count == 0)
        {
            Console.WriteLine("No products to update.");
            return;
        }
        foreach (var p in products)
            Console.WriteLine($"  [{p.Id}] {p.Name}");
        int id = InputHelper.ReadInt("Product ID to update: ", products.Min(p => p.Id), products.Max(p => p.Id));
        string name = InputHelper.ReadString("New name: ");
        string description = InputHelper.ReadString("New description: ");
        decimal price = InputHelper.ReadDecimal("New price: ", 0, 999999.99m);

        if (_productService.Update(id, name, description, price))
            Console.WriteLine("Product updated successfully.");
        else
            Console.WriteLine("Update failed. Product not found or name is empty.");
    }

    private void HandleDeleteProduct()
    {
        var products = _productService.GetAll();
        if (products.Count == 0)
        {
            Console.WriteLine("No products to delete.");
            return;
        }
        foreach (var p in products)
            Console.WriteLine($"  [{p.Id}] {p.Name}");
        int id = InputHelper.ReadInt("Product ID to delete: ", products.Min(p => p.Id), products.Max(p => p.Id));
        if (!InputHelper.ReadYesNo("Delete this product? (y/n): "))
        {
            Console.WriteLine("Cancelled.");
            return;
        }
        if (_productService.Delete(id))
            Console.WriteLine("Product deleted.");
        else
            Console.WriteLine("Product not found.");
    }

    private void HandleRestockProduct()
    {
        var products = _productService.GetAll();
        if (products.Count == 0)
        {
            Console.WriteLine("No products to restock.");
            return;
        }
        foreach (var p in products)
            Console.WriteLine($"  [{p.Id}] {p.Name} (Stock: {p.StockQuantity})");
        int id = InputHelper.ReadInt("Product ID to restock: ", products.Min(p => p.Id), products.Max(p => p.Id));
        int quantity = InputHelper.ReadInt("Quantity to add: ", 1, 100000);

        if (_productService.Restock(id, quantity))
            Console.WriteLine("Product restocked successfully.");
        else
            Console.WriteLine("Restock failed. Product not found.");
    }

    private void HandleViewProducts()
    {
        var products = _productService.GetAll();
        if (products.Count == 0)
        {
            Console.WriteLine("No products.");
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

    private void HandleViewLowStock()
    {
        int threshold = InputHelper.ReadInt($"Low stock threshold (default {LowStockThresholdDefault}): ", 0, 10000);
        var products = _productService.GetLowStock(threshold);
        if (products.Count == 0)
        {
            Console.WriteLine("No products at or below that threshold.");
            return;
        }
        foreach (var p in products)
            Console.WriteLine($"  [{p.Id}] {p.Name} - Stock: {p.StockQuantity}");
    }

    private void HandleViewOrders()
    {
        var orders = _orderService.GetAllOrders();
        if (orders.Count == 0)
        {
            Console.WriteLine("No orders.");
            return;
        }
        foreach (var o in orders)
            Console.WriteLine($"  Order #{o.Id} - Customer #{o.CustomerId} - ${o.TotalAmount:N2} - {o.Status} - {o.CreatedAt:yyyy-MM-dd HH:mm}");
    }

    private void HandleUpdateOrderStatus()
    {
        var orders = _orderService.GetAllOrders();
        if (orders.Count == 0)
        {
            Console.WriteLine("No orders to update.");
            return;
        }
        foreach (var o in orders)
            Console.WriteLine($"  [{o.Id}] Customer #{o.CustomerId} - {o.Status} - ${o.TotalAmount:N2}");
        int orderId = InputHelper.ReadInt("Order ID to update: ", orders.Min(o => o.Id), orders.Max(o => o.Id));
        Console.WriteLine("1. Pending  2. Confirmed  3. Shipped  4. Delivered  5. Cancelled");
        int statusChoice = InputHelper.ReadInt("New status (1-5): ", 1, 5);
        var status = statusChoice switch
        {
            1 => OrderStatus.Pending,
            2 => OrderStatus.Confirmed,
            3 => OrderStatus.Shipped,
            4 => OrderStatus.Delivered,
            5 => OrderStatus.Cancelled,
            _ => OrderStatus.Pending
        };
        if (_orderService.UpdateOrderStatus(orderId, status))
            Console.WriteLine($"Order #{orderId} status updated to {status}.");
        else
            Console.WriteLine("Order not found.");
    }
}
