using SwiftCart.Application.Interfaces;
using SwiftCart.Domain.Entities;

namespace SwiftCart.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepo;

    public ProductService(IProductRepository productRepo)
    {
        _productRepo = productRepo;
    }

    public List<Product> GetAll() => _productRepo.GetAll();

    public Product? GetById(int id) => _productRepo.GetById(id);

    public List<Product> Search(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return new List<Product>();

        return _productRepo.Search(term);
    }

    public List<Product> GetLowStock(int threshold) => _productRepo.GetLowStock(threshold);

    public bool Add(string name, string description, decimal price, int stock)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;
        if (price < 0)
            return false;
        if (stock < 0)
            return false;

        _productRepo.Add(new Product
        {
            Id = _productRepo.GetNextId(),
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

        _productRepo.Remove(product);
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

    public bool DeductStock(int productId, int quantity)
    {
        if (quantity <= 0)
            return false;

        Product? product = GetById(productId);
        if (product == null || product.StockQuantity < quantity)
            return false;

        product.StockQuantity -= quantity;
        return true;
    }
}
