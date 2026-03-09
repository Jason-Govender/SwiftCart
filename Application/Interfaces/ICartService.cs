using SwiftCart.Domain.Entities;

namespace SwiftCart.Application.Interfaces;

public interface ICartService
{
    Cart GetOrCreateCart(int customerId);
    Cart GetCart(int customerId);
    bool AddItem(int customerId, int productId, int quantity);
    bool UpdateItemQuantity(int customerId, int cartItemId, int newQuantity);
    bool RemoveItem(int customerId, int cartItemId);
}
