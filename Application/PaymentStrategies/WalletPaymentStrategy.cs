using SwiftCart.Application.Interfaces;

namespace SwiftCart.Application.PaymentStrategies;

public class WalletPaymentStrategy : IPaymentStrategy
{
    private readonly IWalletService _walletService;

    public WalletPaymentStrategy(IWalletService walletService)
    {
        _walletService = walletService;
    }

    public string MethodName => "Wallet";

    public (bool Success, string? ErrorMessage) Pay(int customerId, decimal amount)
    {
        if (_walletService.GetBalance(customerId) < amount)
            return (false, "Insufficient wallet balance.");

        if (!_walletService.DeductFunds(customerId, amount))
            return (false, "Failed to deduct funds from wallet.");

        return (true, null);
    }

    public void Refund(int customerId, decimal amount)
    {
        _walletService.AddFunds(customerId, amount);
    }
}
