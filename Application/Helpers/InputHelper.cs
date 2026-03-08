namespace SwiftCart.Application.Helpers;

public static class InputHelper
{
    public static string ReadString(string prompt)
    {
        Console.Write(prompt);
        return (Console.ReadLine() ?? string.Empty).Trim();
    }

    public static int ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(input.Trim(), out int value))
            {
                Console.WriteLine("Please enter a valid whole number.");
                continue;
            }
            if (value < min || value > max)
            {
                Console.WriteLine($"Please enter a number between {min} and {max}.");
                continue;
            }
            return value;
        }
    }

    public static decimal ReadDecimal(string prompt, decimal min, decimal max)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine() ?? string.Empty;
            if (!decimal.TryParse(input.Trim(), out decimal value))
            {
                Console.WriteLine("Please enter a valid number.");
                continue;
            }
            if (value < min || value > max)
            {
                Console.WriteLine($"Please enter a number between {min} and {max}.");
                continue;
            }
            return value;
        }
    }

    /// <summary>
    /// Prompts for y/n (or yes/no). Returns true for yes, false for no. Re-prompts until valid input.
    /// </summary>
    public static bool ReadYesNo(string prompt)
    {
        while (true)
        {
            string input = ReadString(prompt).Trim();
            if (input.Equals("y", StringComparison.OrdinalIgnoreCase) ||
                input.Equals("yes", StringComparison.OrdinalIgnoreCase))
                return true;
            if (input.Equals("n", StringComparison.OrdinalIgnoreCase) ||
                input.Equals("no", StringComparison.OrdinalIgnoreCase))
                return false;
            Console.WriteLine("Please enter y or n.");
        }
    }

    public static void WaitForAnyKey(string message)
    {
        Console.WriteLine(message);
        Console.ReadKey(intercept: true);
    }
}
