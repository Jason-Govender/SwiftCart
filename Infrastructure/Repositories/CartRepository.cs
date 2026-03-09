using SwiftCart.Application.Interfaces;
using SwiftCart.Domain.Entities;
using SwiftCart.Infrastructure.Data;

namespace SwiftCart.Infrastructure.Repositories;

public class CartRepository : ICartRepository
{
    private readonly AppDb _db;

    public CartRepository(AppDb db) => _db = db;

    public Cart? GetByCustomerId(int customerId) =>
        _db.Carts.FirstOrDefault(c => c.CustomerId == customerId);

    public void Add(Cart cart) => _db.Carts.Add(cart);

    public int GetNextCartId() =>
        _db.Carts.Count > 0 ? _db.Carts.Max(c => c.Id) + 1 : 1;

    public int GetNextCartItemId()
    {
        int max = _db.Carts
            .Where(c => c.Items != null)
            .SelectMany(c => c.Items)
            .Select(i => i.Id)
            .DefaultIfEmpty(0)
            .Max();
        return max + 1;
    }
}
