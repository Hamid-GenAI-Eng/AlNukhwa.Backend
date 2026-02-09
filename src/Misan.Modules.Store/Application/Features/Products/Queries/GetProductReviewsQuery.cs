using MediatR;
using Microsoft.EntityFrameworkCore;
using Misan.Modules.Store.Infrastructure.Database;
using Misan.Shared.Kernel.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Store.Application.Features.Products.Queries;

public record GetProductReviewsQuery(Guid ProductId) : IRequest<Result<List<ProductReviewDto>>>;

public record ProductReviewDto(Guid Id, Guid UserId, int Rating, string Comment, DateTime CreatedAt);

public class GetProductReviewsQueryHandler : IRequestHandler<GetProductReviewsQuery, Result<List<ProductReviewDto>>>
{
    private readonly StoreDbContext _dbContext;

    public GetProductReviewsQueryHandler(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<List<ProductReviewDto>>> Handle(GetProductReviewsQuery request, CancellationToken cancellationToken)
    {
        var reviews = await _dbContext.ProductReviews
            .Where(r => r.ProductId == request.ProductId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ProductReviewDto(r.Id, r.UserId, r.Rating, r.Comment, r.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result.Success(reviews);
    }
}
