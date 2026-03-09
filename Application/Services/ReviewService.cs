using SwiftCart.Application.Interfaces;
using SwiftCart.Common.Constants;
using SwiftCart.Domain.Entities;

namespace SwiftCart.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepo;
    private readonly IProductService _productService;

    public ReviewService(IReviewRepository reviewRepo, IProductService productService)
    {
        _reviewRepo = reviewRepo;
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

            _reviewRepo.Add(new Review
            {
                Id = _reviewRepo.GetNextId(),
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
            return _reviewRepo.GetByProduct(productId);
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
            return _reviewRepo.GetByCustomerAndProduct(customerId, productId);
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
            var reviews = _reviewRepo.GetByProduct(productId);
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
            return _reviewRepo.CountByProduct(productId);
        }
        catch (Exception)
        {
            return 0;
        }
    }
}
