using SwiftCart.Domain.Entities;

namespace SwiftCart.Application.Interfaces;

public interface IWalletRepository
{
    Wallet? GetByCustomerId(int customerId);
    void Add(Wallet wallet);
    int GetNextId();
}
