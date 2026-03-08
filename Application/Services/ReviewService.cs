using SwiftCart.Domain.Entities;
using SwiftCart.Infrastructure.Data;

namespace SwiftCart.Application.Services;

public class ReviewService
{
    private readonly AppDb _db;
    private readonly ProductService _productService;

    public ReviewService(AppDb db, ProductService productService)
    {
        _db = db;
        _productService = productService;
    }

    /// <summary>
    /// Submits a new review or updates the existing review for this customer and product.
    /// Returns (true, null) on success, or (false, errorMessage) on failure.
    /// </summary>
    public (bool Success, string? ErrorMessage) SubmitOrUpdateReview(int customerId, int productId, int rating, string? comment)
    {
        if (_productService.GetById(productId) == null)
            return (false, "Invalid product.");

        if (rating < 1 || rating > 5)
            return (false, "Rating must be between 1 and 5.");

        string commentText = (comment ?? string.Empty).Trim();
        var existing = GetReviewByCustomerAndProduct(customerId, productId);

        if (existing != null)
        {
            existing.Rating = rating;
            existing.Comment = commentText;
            return (true, null);
        }

        _db.Reviews.Add(new Review
        {
            Id = GetNextReviewId(),
            CustomerId = customerId,
            ProductId = productId,
            Rating = rating,
            Comment = commentText,
            CreatedAt = DateTime.UtcNow
        });
        return (true, null);
    }

    public List<Review> GetReviewsByProduct(int productId)
    {
        return _db.Reviews
            .Where(r => r.ProductId == productId)
            .OrderByDescending(r => r.CreatedAt)
            .ToList();
    }

    public Review? GetReviewByCustomerAndProduct(int customerId, int productId)
    {
        return _db.Reviews
            .FirstOrDefault(r => r.CustomerId == customerId && r.ProductId == productId);
    }

    public double? GetAverageRating(int productId)
    {
        var reviews = _db.Reviews.Where(r => r.ProductId == productId).ToList();
        if (reviews.Count == 0)
            return null;
        return reviews.Average(r => r.Rating);
    }

    public int GetReviewCount(int productId)
    {
        return _db.Reviews.Count(r => r.ProductId == productId);
    }

    private int GetNextReviewId()
    {
        if (_db.Reviews.Count == 0)
            return 1;
        return _db.Reviews.Max(r => r.Id) + 1;
    }
}
