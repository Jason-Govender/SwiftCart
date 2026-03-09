using SwiftCart.Application.Interfaces;
using SwiftCart.Domain.Entities;
using SwiftCart.Infrastructure.Data;

namespace SwiftCart.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDb _db;

    public ProductRepository(AppDb db) => _db = db;

    public List<Product> GetAll() => _db.Products.ToList();

    public Product? GetById(int id) => _db.Products.FirstOrDefault(p => p.Id == id);

    public List<Product> Search(string term)
    {
        string lower = term.Trim().ToLowerInvariant();
        return _db.Products
            .Where(p => (p.Name + " " + p.Description).ToLowerInvariant().Contains(lower))
            .ToList();
    }

    public List<Product> GetLowStock(int threshold) =>
        _db.Products.Where(p => p.StockQuantity <= threshold).ToList();

    public void Add(Product product) => _db.Products.Add(product);

    public void Remove(Product product) => _db.Products.Remove(product);

    public int GetNextId() =>
        _db.Products.Count > 0 ? _db.Products.Max(p => p.Id) + 1 : 1;
}
