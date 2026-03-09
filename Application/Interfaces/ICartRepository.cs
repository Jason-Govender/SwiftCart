using SwiftCart.Domain.Entities;

namespace SwiftCart.Application.Interfaces;

public interface ICartRepository
{
    Cart? GetByCustomerId(int customerId);
    void Add(Cart cart);
    int GetNextCartId();
    int GetNextCartItemId();
}
