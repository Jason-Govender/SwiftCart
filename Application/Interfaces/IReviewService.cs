using SwiftCart.Domain.Entities;

namespace SwiftCart.Application.Interfaces;

public interface IReviewService
{
    (bool Success, string? ErrorMessage) SubmitOrUpdateReview(int customerId, int productId, int rating, string? comment);
    List<Review> GetReviewsByProduct(int productId);
    Review? GetReviewByCustomerAndProduct(int customerId, int productId);
    double? GetAverageRating(int productId);
    int GetReviewCount(int productId);
}
