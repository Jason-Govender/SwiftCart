using SwiftCart.Domain.Entities;
using SwiftCart.Infrastructure.Data;

namespace SwiftCart.Application.Services;

public class ProductService
{
    private readonly AppDb _db;

    public ProductService(AppDb db)
    {
        _db = db;
    }

    public List<Product> GetAll()
    {
        return _db.Products.ToList();
    }

    public Product? GetById(int id)
    {
        return _db.Products.FirstOrDefault(p => p.Id == id);
    }

    public List<Product> Search(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return new List<Product>();

        string lower = term.Trim().ToLowerInvariant();
        return _db.Products
            .Where(p => (p.Name + " " + p.Description).ToLowerInvariant().Contains(lower))
            .ToList();
    }

    public List<Product> GetLowStock(int threshold)
    {
        return _db.Products.Where(p => p.StockQuantity <= threshold).ToList();
    }

    public bool Add(string name, string description, decimal price, int stock)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;
        if (price < 0)
            return false;
        if (stock < 0)
            return false;

        int nextId = _db.Products.Count > 0 ? _db.Products.Max(p => p.Id) + 1 : 1;
        _db.Products.Add(new Product
        {
            Id = nextId,
            Name = name.Trim(),
            Description = description?.Trim() ?? string.Empty,
            Price = price,
            StockQuantity = stock
        });
        return true;
    }

    public bool Update(int id, string name, string description, decimal price)
    {
        Product? product = GetById(id);
        if (product == null)
            return false;
        if (string.IsNullOrWhiteSpace(name))
            return false;
        if (price < 0)
            return false;

        product.Name = name.Trim();
        product.Description = description?.Trim() ?? string.Empty;
        product.Price = price;
        return true;
    }

    public bool Delete(int id)
    {
        Product? product = GetById(id);
        if (product == null)
            return false;

        _db.Products.Remove(product);
        return true;
    }

    public bool Restock(int id, int quantity)
    {
        if (quantity <= 0)
            return false;

        Product? product = GetById(id);
        if (product == null)
            return false;

        product.StockQuantity += quantity;
        return true;
    }
}
