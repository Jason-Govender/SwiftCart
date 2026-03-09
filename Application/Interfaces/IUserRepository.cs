using SwiftCart.Domain.Entities;

namespace SwiftCart.Application.Interfaces;

public interface IUserRepository
{
    bool ExistsWithUsername(string username);
    int GetNextId();
    void Add(User user);
    User? FindByCredentials(string username, string password);
}
