using MediatR;
using Microsoft.EntityFrameworkCore;
using Misan.Modules.Store.Domain.Entities;
using Misan.Modules.Store.Infrastructure.Database;
using Misan.Shared.Kernel.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Store.Application.Features.Wishlist.Commands;

public record AddToWishlistCommand(Guid UserId, string ItemType, Guid ItemId) : IRequest<Result>;
public record RemoveFromWishlistCommand(Guid UserId, Guid ItemId) : IRequest<Result>;
public record GetWishlistQuery(Guid UserId) : IRequest<Result<List<WishlistItemDto>>>;
public record WishlistItemDto(Guid Id, string Type, Guid ItemId, DateTime AddedAt);

public class WishlistHandlers : 
    IRequestHandler<AddToWishlistCommand, Result>,
    IRequestHandler<RemoveFromWishlistCommand, Result>,
    IRequestHandler<GetWishlistQuery, Result<List<WishlistItemDto>>>
{
    private readonly StoreDbContext _dbContext;

    public WishlistHandlers(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(AddToWishlistCommand request, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.Wishlists.AnyAsync(w => w.UserId == request.UserId && w.ItemId == request.ItemId, cancellationToken);
        if (exists) return Result.Success(); 

        var item = Domain.Entities.Wishlist.Create(request.UserId, request.ItemType, request.ItemId);
        _dbContext.Wishlists.Add(item);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> Handle(RemoveFromWishlistCommand request, CancellationToken cancellationToken)
    {
        var item = await _dbContext.Wishlists.FirstOrDefaultAsync(w => w.UserId == request.UserId && w.ItemId == request.ItemId, cancellationToken);
        if (item != null)
        {
            _dbContext.Wishlists.Remove(item);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        return Result.Success();
    }

    public async Task<Result<List<WishlistItemDto>>> Handle(GetWishlistQuery request, CancellationToken cancellationToken)
    {
        var items = await _dbContext.Wishlists
            .Where(w => w.UserId == request.UserId)
            .OrderByDescending(w => w.CreatedAt)
            .Select(w => new WishlistItemDto(w.Id, w.ItemType, w.ItemId, w.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<List<WishlistItemDto>>.Success(items);
    }
}
