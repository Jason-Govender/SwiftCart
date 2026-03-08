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

    public bool Register(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return false;

        if (_db.Users.Any(u => u.Username.Equals(username.Trim(), StringComparison.OrdinalIgnoreCase)))
            return false;

        int nextId = _db.Users.Count > 0 ? _db.Users.Max(u => u.Id) + 1 : 1;
        Customer customer = new Customer
        {
            Id = nextId,
            Username = username.Trim(),
            Password = password
        };
        _db.Users.Add(customer);
        return true;
    }
}
