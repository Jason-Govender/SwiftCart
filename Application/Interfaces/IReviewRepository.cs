using SwiftCart.Domain.Entities;

namespace SwiftCart.Application.Interfaces;

public interface IReviewRepository
{
    void Add(Review review);
    List<Review> GetByProduct(int productId);
    Review? GetByCustomerAndProduct(int customerId, int productId);
    int CountByProduct(int productId);
    int GetNextId();
}
