using SwiftCart.Common.Constants;
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
        try
        {
            if (customerId <= 0)
                return (false, "Invalid customer.");

            if (productId <= 0)
                return (false, "Invalid product.");

            if (_productService.GetById(productId) == null)
                return (false, "Product not found.");

            if (rating < 1 || rating > 5)
                return (false, "Rating must be between 1 and 5.");

            string commentText = (comment ?? string.Empty).Trim();
            if (commentText.Length > AppConstants.MaxReviewCommentLength)
                return (false, $"Comment cannot exceed {AppConstants.MaxReviewCommentLength} characters.");

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
        catch (Exception)
        {
            return (false, "An error occurred while saving your review. Please try again.");
        }
    }

    public List<Review> GetReviewsByProduct(int productId)
    {
        try
        {
            return _db.Reviews
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();
        }
        catch (Exception)
        {
            return new List<Review>();
        }
    }

    public Review? GetReviewByCustomerAndProduct(int customerId, int productId)
    {
        try
        {
            return _db.Reviews
                .FirstOrDefault(r => r.CustomerId == customerId && r.ProductId == productId);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public double? GetAverageRating(int productId)
    {
        try
        {
            var reviews = _db.Reviews.Where(r => r.ProductId == productId).ToList();
            if (reviews.Count == 0)
                return null;
            return reviews.Average(r => r.Rating);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public int GetReviewCount(int productId)
    {
        try
        {
            return _db.Reviews.Count(r => r.ProductId == productId);
        }
        catch (Exception)
        {
            return 0;
        }
    }

    private int GetNextReviewId()
    {
        if (_db.Reviews.Count == 0)
            return 1;
        return _db.Reviews.Max(r => r.Id) + 1;
    }
}
