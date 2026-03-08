namespace SwiftCart.Application.Helpers;

public static class InputHelper
{
    public static string ReadString(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine() ?? string.Empty;
    }

    public static int ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int value) && value >= min && value <= max)
                return value;
            Console.WriteLine($"Please enter a number between {min} and {max}.");
        }
    }

    public static decimal ReadDecimal(string prompt, decimal min, decimal max)
    {
        while (true)
        {
            Console.Write(prompt);
            if (decimal.TryParse(Console.ReadLine(), out decimal value) && value >= min && value <= max)
                return value;
            Console.WriteLine($"Please enter a number between {min} and {max}.");
        }
    }

    public static void WaitForAnyKey(string message)
    {
        Console.WriteLine(message);
        Console.ReadKey(intercept: true);
    }
}
