using SwiftCart.Domain.Entities;

namespace SwiftCart.Application.Interfaces;

public interface IWalletService
{
    Wallet GetOrCreateWallet(int customerId);
    decimal GetBalance(int customerId);
    bool AddFunds(int customerId, decimal amount);
    bool DeductFunds(int customerId, decimal amount);
}
