using SwiftCart.Domain.Entities;

namespace SwiftCart.Infrastructure.Data;

public class AppDb
{
    public List<User> Users { get; } = new();
}
