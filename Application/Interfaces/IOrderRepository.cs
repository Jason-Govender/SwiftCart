using SwiftCart.Domain.Entities;

namespace SwiftCart.Application.Interfaces;

public interface IOrderRepository
{
    void Add(Order order);
    Order? GetById(int orderId);
    List<Order> GetByCustomer(int customerId);
    List<Order> GetAll();
    List<Order> GetNonCancelled();
    int GetNextId();
    int GetNextItemId();
}
