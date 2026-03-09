using SwiftCart.Application.Interfaces;
using SwiftCart.Domain.Entities;
using SwiftCart.Infrastructure.Data;

namespace SwiftCart.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDb _db;

    public UserRepository(AppDb db) => _db = db;

    public bool ExistsWithUsername(string username) =>
        _db.Users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

    public int GetNextId() =>
        _db.Users.Count > 0 ? _db.Users.Max(u => u.Id) + 1 : 1;

    public void Add(User user) => _db.Users.Add(user);

    public User? FindByCredentials(string username, string password) =>
        _db.Users.FirstOrDefault(u =>
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && u.Password == password);
}
