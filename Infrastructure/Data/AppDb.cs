using SwiftCart.Domain.Entities;

namespace SwiftCart.Infrastructure.Data;

public class AppDb
{
    public List<User> Users { get; } = new();
    public List<Product> Products { get; } = new();
    public List<Cart> Carts { get; } = new();
    public List<Wallet> Wallets { get; } = new();
    public List<Order> Orders { get; } = new();
    public List<Review> Reviews { get; } = new();
    public List<Payment> Payments { get; } = new();
    public List<Notification> Notifications { get; } = new();
}
