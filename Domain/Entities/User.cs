using SwiftCart.Domain.Enums;

namespace SwiftCart.Domain.Entities;

public abstract class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public abstract UserRole Role { get; }
}
