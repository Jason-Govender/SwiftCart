using SwiftCart.Application.Interfaces;
using SwiftCart.Domain.Entities;

namespace SwiftCart.Application.Services;

public class WalletService : IWalletService
{
    private readonly IWalletRepository _walletRepo;

    public WalletService(IWalletRepository walletRepo)
    {
        _walletRepo = walletRepo;
    }

    public Wallet GetOrCreateWallet(int customerId)
    {
        var wallet = _walletRepo.GetByCustomerId(customerId);
        if (wallet != null)
            return wallet;

        wallet = new Wallet
        {
            Id = _walletRepo.GetNextId(),
            CustomerId = customerId,
            Balance = 0
        };
        _walletRepo.Add(wallet);
        return wallet;
    }

    public decimal GetBalance(int customerId)
    {
        var wallet = _walletRepo.GetByCustomerId(customerId);
        return wallet?.Balance ?? 0;
    }

    public bool AddFunds(int customerId, decimal amount)
    {
        if (amount <= 0)
            return false;

        var wallet = GetOrCreateWallet(customerId);
        wallet.Balance += amount;
        return true;
    }

    public bool DeductFunds(int customerId, decimal amount)
    {
        if (amount <= 0)
            return false;

        var wallet = _walletRepo.GetByCustomerId(customerId);
        if (wallet == null || wallet.Balance < amount)
            return false;

        wallet.Balance -= amount;
        return true;
    }
}
