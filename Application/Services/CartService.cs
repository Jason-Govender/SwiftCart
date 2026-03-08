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
        {
            EnsureCartItemsInitialized(cart);
            return cart;
        }

        cart = new Cart
        {
            Id = GetNextCartId(),
            CustomerId = customerId,
            Items = new List<CartItem>()
        };
        _db.Carts.Add(cart);
        return cart;
    }

    private static int GetNextCartId(AppDb db)
    {
        if (db.Carts == null || db.Carts.Count == 0)
            return 1;
        return db.Carts.Max(c => c.Id) + 1;
    }

    private int GetNextCartId() => GetNextCartId(_db);

    private static int GetNextCartItemId(AppDb db)
    {
        if (db.Carts == null)
            return 1;
        int max = db.Carts
            .Where(c => c.Items != null)
            .SelectMany(c => c.Items)
            .Select(i => i.Id)
            .DefaultIfEmpty(0)
            .Max();
        return max + 1;
    }

    private int GetNextCartItemId() => GetNextCartItemId(_db);

    private static void EnsureCartItemsInitialized(Cart cart)
    {
        if (cart.Items == null)
            cart.Items = new List<CartItem>();
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
            EnsureCartItemsInitialized(cart);
            cart.Items.Add(new CartItem
            {
                Id = GetNextCartItemId(),
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
        if (product == null)
            newQuantity = Math.Min(newQuantity, item.Quantity);
        else if (newQuantity > product.StockQuantity)
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
