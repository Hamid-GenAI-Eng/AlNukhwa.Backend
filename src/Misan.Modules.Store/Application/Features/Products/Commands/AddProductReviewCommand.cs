using FluentValidation;
using MediatR;
using Misan.Modules.Store.Domain.Entities;
using Misan.Modules.Store.Infrastructure.Database;
using Misan.Shared.Kernel.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Store.Application.Features.Products.Commands;

public record AddProductReviewCommand(Guid UserId, Guid ProductId, int Rating, string Comment) : IRequest<Result>;

public class AddProductReviewCommandHandler : IRequestHandler<AddProductReviewCommand, Result>
{
    private readonly StoreDbContext _dbContext;

    public AddProductReviewCommandHandler(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(AddProductReviewCommand request, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products.FindAsync(new object[] { request.ProductId }, cancellationToken);
        if (product == null) return Result.Failure(new Error("Product.NotFound", "Product not found"));

        var review = ProductReview.Create(request.UserId, request.ProductId, request.Rating, request.Comment);
        _dbContext.ProductReviews.Add(review);
        
        // TODO: Update product average rating? For now just adding review.
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
