using SwiftCart.Domain.Entities;
using SwiftCart.Domain.Enums;

namespace SwiftCart.Domain.Factories;

public interface IUserFactory
{
    User Create(UserRole role, int id, string username, string password);
}
