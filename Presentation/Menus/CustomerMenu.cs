using SwiftCart.Application.Helpers;
using SwiftCart.Application.Services;
using SwiftCart.Domain.Entities;

namespace SwiftCart.Presentation.Menus;

public class CustomerMenu
{
    private readonly AuthService _authService;

    public CustomerMenu(AuthService authService)
    {
        _authService = authService;
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
                return;
            }

            Console.WriteLine("Not implemented yet.");
        }
    }
}
