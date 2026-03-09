using SwiftCart.Domain.Entities;

namespace SwiftCart.Application.Interfaces;

public interface IProductRepository
{
    List<Product> GetAll();
    Product? GetById(int id);
    List<Product> Search(string term);
    List<Product> GetLowStock(int threshold);
    void Add(Product product);
    void Remove(Product product);
    int GetNextId();
}
