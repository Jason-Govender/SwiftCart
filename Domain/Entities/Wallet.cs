namespace SwiftCart.Domain.Entities;

public class Wallet
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public decimal Balance { get; set; }
}
