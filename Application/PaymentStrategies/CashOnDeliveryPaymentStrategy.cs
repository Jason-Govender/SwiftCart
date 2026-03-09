using SwiftCart.Application.Interfaces;

namespace SwiftCart.Application.PaymentStrategies;

public class CashOnDeliveryPaymentStrategy : IPaymentStrategy
{
    public string MethodName => "Cash on Delivery";

    public (bool Success, string? ErrorMessage) Pay(int customerId, decimal amount)
    {
        return (true, null);
    }

    public void Refund(int customerId, decimal amount)
    {
        // No upfront charge was made, so no refund is needed.
    }
}
