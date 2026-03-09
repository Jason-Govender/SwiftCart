namespace SwiftCart.Application.Interfaces;

public interface IPaymentStrategy
{
    string MethodName { get; }
    (bool Success, string? ErrorMessage) Pay(int customerId, decimal amount);
    void Refund(int customerId, decimal amount);
}
