using MediatR;
using Microsoft.EntityFrameworkCore;
using Misan.Modules.Store.Infrastructure.Database;
using Misan.Shared.Kernel.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Store.Application.Features.Wishlist.Queries;

public record CheckWishlistStatusQuery(Guid UserId, string ItemType, Guid ItemId) : IRequest<Result<bool>>;

public class CheckWishlistStatusQueryHandler : IRequestHandler<CheckWishlistStatusQuery, Result<bool>>
{
    private readonly StoreDbContext _dbContext;
    public CheckWishlistStatusQueryHandler(StoreDbContext dbContext) => _dbContext = dbContext;

    public async Task<Result<bool>> Handle(CheckWishlistStatusQuery request, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.Wishlists
            .AnyAsync(w => w.UserId == request.UserId && w.ItemType == request.ItemType && w.ItemId == request.ItemId, cancellationToken);
        return Result.Success(exists);
    }
}
