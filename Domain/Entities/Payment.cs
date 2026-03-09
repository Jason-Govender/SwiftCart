namespace SwiftCart.Domain.Entities;

public class Payment
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = "Wallet";
    public DateTime PaidAt { get; set; }
}
