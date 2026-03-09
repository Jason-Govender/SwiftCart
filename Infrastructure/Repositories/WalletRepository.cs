using SwiftCart.Application.Interfaces;
using SwiftCart.Domain.Entities;
using SwiftCart.Infrastructure.Data;

namespace SwiftCart.Infrastructure.Repositories;

public class WalletRepository : IWalletRepository
{
    private readonly AppDb _db;

    public WalletRepository(AppDb db) => _db = db;

    public Wallet? GetByCustomerId(int customerId) =>
        _db.Wallets.FirstOrDefault(w => w.CustomerId == customerId);

    public void Add(Wallet wallet) => _db.Wallets.Add(wallet);

    public int GetNextId() =>
        _db.Wallets.Count > 0 ? _db.Wallets.Max(w => w.Id) + 1 : 1;
}
