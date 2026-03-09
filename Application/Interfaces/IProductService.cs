using SwiftCart.Domain.Entities;

namespace SwiftCart.Application.Interfaces;

public interface IProductService
{
    List<Product> GetAll();
    Product? GetById(int id);
    List<Product> Search(string term);
    List<Product> GetLowStock(int threshold);
    bool Add(string name, string description, decimal price, int stock);
    bool Update(int id, string name, string description, decimal price);
    bool Delete(int id);
    bool Restock(int id, int quantity);
    bool DeductStock(int productId, int quantity);
}
