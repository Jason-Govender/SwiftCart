using SwiftCart.Domain.Entities;
using SwiftCart.Domain.Enums;

namespace SwiftCart.Domain.Factories;

public class UserFactory : IUserFactory
{
    public User Create(UserRole role, int id, string username, string password) => role switch
    {
        UserRole.Customer      => new Customer      { Id = id, Username = username, Password = password },
        UserRole.Administrator => new Administrator { Id = id, Username = username, Password = password },
        _ => throw new ArgumentOutOfRangeException(nameof(role), $"Unknown role: {role}")
    };
}
