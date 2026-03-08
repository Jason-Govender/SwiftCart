using SwiftCart.Application.Helpers;
using SwiftCart.Domain.Entities;

namespace SwiftCart.Presentation.Menus;

public class AdministratorMenu
{
    public void Run(Administrator user)
    {
        while (true)
        {
            Console.WriteLine($"\n--- Administrator Menu (logged in as {user.Username}) ---");
            Console.WriteLine("1. Add Product");
            Console.WriteLine("2. Update Product");
            Console.WriteLine("3. Delete Product");
            Console.WriteLine("4. Restock Product");
            Console.WriteLine("5. View Products");
            Console.WriteLine("6. View Orders");
            Console.WriteLine("7. Update Order Status");
            Console.WriteLine("8. View Low Stock Products");
            Console.WriteLine("9. Generate Sales Reports");
            Console.WriteLine("10. Logout");

            int choice = InputHelper.ReadInt("Select option: ", 1, 10);

            if (choice == 10)
            {
                return;
            }

            Console.WriteLine("Not implemented yet.");
        }
    }
}
