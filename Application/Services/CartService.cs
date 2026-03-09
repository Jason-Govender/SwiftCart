using SwiftCart.Application.Interfaces;
using SwiftCart.Domain.Entities;

namespace SwiftCart.Application.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepo;
    private readonly IProductRepository _productRepo;

    public CartService(ICartRepository cartRepo, IProductRepository productRepo)
    {
        _cartRepo = cartRepo;
        _productRepo = productRepo;
    }

    public Cart GetOrCreateCart(int customerId)
    {
        var cart = _cartRepo.GetByCustomerId(customerId);
        if (cart != null)
        {
            EnsureCartItemsInitialized(cart);
            return cart;
        }

        cart = new Cart
        {
            Id = _cartRepo.GetNextCartId(),
            CustomerId = customerId,
            Items = new List<CartItem>()
        };
        _cartRepo.Add(cart);
        return cart;
    }

    private static void EnsureCartItemsInitialized(Cart cart)
    {
        if (cart.Items == null)
            cart.Items = new List<CartItem>();
    }

    public Cart GetCart(int customerId) => GetOrCreateCart(customerId);

    public bool AddItem(int customerId, int productId, int quantity)
    {
        if (quantity <= 0)
            return false;

        var product = _productRepo.GetById(productId);
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
                Id = _cartRepo.GetNextCartItemId(),
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
        var cart = _cartRepo.GetByCustomerId(customerId);
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

        var product = _productRepo.GetById(item.ProductId);
        if (product == null)
            newQuantity = Math.Min(newQuantity, item.Quantity);
        else if (newQuantity > product.StockQuantity)
            newQuantity = product.StockQuantity;

        item.Quantity = newQuantity;
        if (product != null)
            item.UnitPrice = product.Price;
        return true;
    }

    public bool RemoveItem(int customerId, int cartItemId) =>
        UpdateItemQuantity(customerId, cartItemId, 0);
}
