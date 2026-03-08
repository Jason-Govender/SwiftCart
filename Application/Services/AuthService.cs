using SwiftCart.Application.Enums;
using SwiftCart.Domain.Entities;
using SwiftCart.Infrastructure.Data;

namespace SwiftCart.Application.Services;

public class AuthService
{
    private readonly AppDb _db;

    public AuthService(AppDb db)
    {
        _db = db;
    }

    public RegistrationResult Register(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return RegistrationResult.EmptyCredentials;

        if (!MeetsPasswordStrength(password))
            return RegistrationResult.WeakPassword;

        if (_db.Users.Any(u => u.Username.Equals(username.Trim(), StringComparison.OrdinalIgnoreCase)))
            return RegistrationResult.DuplicateUsername;

        int nextId = _db.Users.Count > 0 ? _db.Users.Max(u => u.Id) + 1 : 1;
        Customer customer = new Customer
        {
            Id = nextId,
            Username = username.Trim(),
            Password = password
        };
        _db.Users.Add(customer);
        return RegistrationResult.Success;
    }

    private static bool MeetsPasswordStrength(string password)
    {
        if (password.Length < 8) return false;
        if (!password.Any(char.IsUpper)) return false;
        if (!password.Any(char.IsLower)) return false;
        if (!password.Any(char.IsDigit)) return false;
        if (!password.Any(c => !char.IsLetterOrDigit(c))) return false;
        return true;
    }
}
