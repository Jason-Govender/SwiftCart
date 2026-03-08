using SwiftCart.Domain.Entities;
using SwiftCart.Infrastructure.Data;

namespace SwiftCart.Application.Services;

public class CartService
{
    private readonly AppDb _db;

    public CartService(AppDb db)
    {
        _db = db;
    }

    public Cart GetOrCreateCart(int customerId)
    {
        var cart = _db.Carts.FirstOrDefault(c => c.CustomerId == customerId);
        if (cart != null)
            return cart;

        int nextId = _db.Carts.Count > 0 ? _db.Carts.Max(c => c.Id) + 1 : 1;
        cart = new Cart
        {
            Id = nextId,
            CustomerId = customerId,
            Items = new List<CartItem>()
        };
        _db.Carts.Add(cart);
        return cart;
    }

    public Cart GetCart(int customerId)
    {
        return GetOrCreateCart(customerId);
    }

    public bool AddItem(int customerId, int productId, int quantity)
    {
        if (quantity <= 0)
            return false;

        var product = _db.Products.FirstOrDefault(p => p.Id == productId);
        if (product == null)
            return false;
        if (product.StockQuantity < quantity)
            return false;

        var cart = GetOrCreateCart(customerId);
        var existing = cart.Items.FirstOrDefault(i => i.ProductId == productId);

        if (existing != null)
        {
            if (product.StockQuantity < existing.Quantity + quantity)
                return false;
            existing.Quantity += quantity;
            existing.UnitPrice = product.Price;
        }
        else
        {
            int nextItemId = _db.Carts.SelectMany(c => c.Items).Select(i => i.Id).DefaultIfEmpty(0).Max() + 1;
            cart.Items.Add(new CartItem
            {
                Id = nextItemId,
                CartId = cart.Id,
                ProductId = productId,
                Quantity = quantity,
                UnitPrice = product.Price
            });
        }

        return true;
    }

    public bool UpdateItemQuantity(int customerId, int cartItemId, int newQuantity)
    {
        var cart = _db.Carts.FirstOrDefault(c => c.CustomerId == customerId);
        if (cart == null)
            return false;

        var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);
        if (item == null)
            return false;

        if (newQuantity <= 0)
        {
            cart.Items.Remove(item);
            return true;
        }

        var product = _db.Products.FirstOrDefault(p => p.Id == item.ProductId);
        if (product != null && newQuantity > product.StockQuantity)
            newQuantity = product.StockQuantity;

        item.Quantity = newQuantity;
        if (product != null)
            item.UnitPrice = product.Price;
        return true;
    }

    public bool RemoveItem(int customerId, int cartItemId)
    {
        return UpdateItemQuantity(customerId, cartItemId, 0);
    }
}
