using SwiftCart.Application.Enums;
using SwiftCart.Application.Helpers;
using SwiftCart.Application.Services;

namespace SwiftCart.Presentation.Menus;

public class MainMenu
{
    private readonly AuthService _authService;

    public MainMenu(AuthService authService)
    {
        _authService = authService;
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
                    Console.WriteLine("Login not implemented yet.");
                    break;
                case 3:
                    Console.WriteLine("Goodbye!");
                    return;
            }
        }
    }

    private void HandleRegister()
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
}
