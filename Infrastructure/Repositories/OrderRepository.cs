using SwiftCart.Application.Interfaces;
using SwiftCart.Domain.Entities;
using SwiftCart.Domain.Enums;
using SwiftCart.Infrastructure.Data;

namespace SwiftCart.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDb _db;

    public OrderRepository(AppDb db) => _db = db;

    public void Add(Order order) => _db.Orders.Add(order);

    public Order? GetById(int orderId) =>
        _db.Orders.FirstOrDefault(o => o.Id == orderId);

    public List<Order> GetByCustomer(int customerId) =>
        _db.Orders
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedAt)
            .ToList();

    public List<Order> GetAll() =>
        _db.Orders.OrderByDescending(o => o.CreatedAt).ToList();

    public List<Order> GetNonCancelled() =>
        _db.Orders.Where(o => o.Status != OrderStatus.Cancelled).ToList();

    public int GetNextId() =>
        _db.Orders.Count > 0 ? _db.Orders.Max(o => o.Id) + 1 : 1;

    public int GetNextItemId()
    {
        int max = _db.Orders
            .Where(o => o.Items != null)
            .SelectMany(o => o.Items)
            .Select(i => i.Id)
            .DefaultIfEmpty(0)
            .Max();
        return max + 1;
    }
}
