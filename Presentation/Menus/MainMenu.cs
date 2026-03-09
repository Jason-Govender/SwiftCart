using SwiftCart.Application.Enums;
using SwiftCart.Application.Helpers;
using SwiftCart.Application.Interfaces;
using SwiftCart.Domain.Entities;

namespace SwiftCart.Presentation.Menus;

public class MainMenu
{
    private readonly IAuthService _authService;
    private readonly CustomerMenu _customerMenu;
    private readonly AdministratorMenu _administratorMenu;

    public MainMenu(IAuthService authService, CustomerMenu customerMenu, AdministratorMenu administratorMenu)
    {
        _authService = authService;
        _customerMenu = customerMenu;
        _administratorMenu = administratorMenu;
    }

    public void Run()
    {
        while (true)
        {
            Console.WriteLine("\n--- Main Menu ---");
            Console.WriteLine("1. Register");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Exit");

            int choice = InputHelper.ReadInt("Select option: ", 1, 3);

            switch (choice)
            {
                case 1:
                    HandleRegister();
                    break;
                case 2:
                    HandleLogin();
                    break;
                case 3:
                    Console.WriteLine("Goodbye!");
                    return;
            }
        }
    }

    private void HandleRegister()
    {
        try
        {
            string username = InputHelper.ReadString("Username: ");
            string password = InputHelper.ReadString("Password: ");

            RegistrationResult result = _authService.Register(username, password);

            switch (result)
            {
                case RegistrationResult.Success:
                    Console.WriteLine("Registration successful.");
                    break;
                case RegistrationResult.EmptyCredentials:
                    Console.WriteLine("Registration failed. Username and password are required.");
                    break;
                case RegistrationResult.WeakPassword:
                    Console.WriteLine("Registration failed. Password must be at least 8 characters with uppercase, lowercase, number, and symbol.");
                    break;
                case RegistrationResult.DuplicateUsername:
                    Console.WriteLine("Registration failed. That username is already taken.");
                    break;
            }
        }
        catch (Exception)
        {
            Console.WriteLine("An unexpected error occurred. Please try again.");
        }
    }

    private void HandleLogin()
    {
        try
        {
            string username = InputHelper.ReadString("Username: ");
            string password = InputHelper.ReadString("Password: ");

            User? user = _authService.Login(username, password);

            if (user == null)
            {
                Console.WriteLine("Login failed. Invalid username or password.");
                return;
            }

            if (user is Customer customer)
                _customerMenu.Run(customer);
            else if (user is Administrator admin)
                _administratorMenu.Run(admin);
        }
        catch (Exception)
        {
            Console.WriteLine("An unexpected error occurred. Please try again.");
        }
    }
}
