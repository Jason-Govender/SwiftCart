using SwiftCart.Application.Interfaces;
using SwiftCart.Domain.Entities;
using SwiftCart.Infrastructure.Data;

namespace SwiftCart.Infrastructure.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly AppDb _db;

    public ReviewRepository(AppDb db) => _db = db;

    public void Add(Review review) => _db.Reviews.Add(review);

    public List<Review> GetByProduct(int productId) =>
        _db.Reviews
            .Where(r => r.ProductId == productId)
            .OrderByDescending(r => r.CreatedAt)
            .ToList();

    public Review? GetByCustomerAndProduct(int customerId, int productId) =>
        _db.Reviews.FirstOrDefault(r => r.CustomerId == customerId && r.ProductId == productId);

    public int CountByProduct(int productId) =>
        _db.Reviews.Count(r => r.ProductId == productId);

    public int GetNextId() =>
        _db.Reviews.Count > 0 ? _db.Reviews.Max(r => r.Id) + 1 : 1;
}
