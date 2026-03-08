using SwiftCart.Domain.Entities;
using SwiftCart.Infrastructure.Data;

namespace SwiftCart.Application.Services;

public class WalletService
{
    private readonly AppDb _db;

    public WalletService(AppDb db)
    {
        _db = db;
    }

    public Wallet GetOrCreateWallet(int customerId)
    {
        var wallet = _db.Wallets.FirstOrDefault(w => w.CustomerId == customerId);
        if (wallet != null)
            return wallet;

        wallet = new Wallet
        {
            Id = GetNextWalletId(),
            CustomerId = customerId,
            Balance = 0
        };
        _db.Wallets.Add(wallet);
        return wallet;
    }

    public decimal GetBalance(int customerId)
    {
        var wallet = _db.Wallets.FirstOrDefault(w => w.CustomerId == customerId);
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

        var wallet = _db.Wallets.FirstOrDefault(w => w.CustomerId == customerId);
        if (wallet == null || wallet.Balance < amount)
            return false;

        wallet.Balance -= amount;
        return true;
    }

    private static int GetNextWalletId(AppDb db)
    {
        if (db.Wallets == null || db.Wallets.Count == 0)
            return 1;
        return db.Wallets.Max(w => w.Id) + 1;
    }

    private int GetNextWalletId() => GetNextWalletId(_db);
}
