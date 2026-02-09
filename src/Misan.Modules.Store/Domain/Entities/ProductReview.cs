using Misan.Shared.Kernel.Abstractions;
using System;

namespace Misan.Modules.Store.Domain.Entities;

public sealed class ProductReview : Entity
{
    private ProductReview(Guid id, Guid userId, Guid productId, int rating, string comment) : base(id)
    {
        UserId = userId;
        ProductId = productId;
        Rating = rating;
        Comment = comment;
        CreatedAt = DateTime.UtcNow;
    }

    private ProductReview() { }

    public Guid UserId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Rating { get; private set; } // 1-5
    public string Comment { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    public static ProductReview Create(Guid userId, Guid productId, int rating, string comment)
    {
        if (rating < 1 || rating > 5) throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5.");
        return new ProductReview(Guid.NewGuid(), userId, productId, rating, comment);
    }
}
