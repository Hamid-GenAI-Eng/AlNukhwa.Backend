using MediatR;
using Microsoft.EntityFrameworkCore;
using Misan.Modules.Store.Infrastructure.Database;
using Misan.Shared.Kernel.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Store.Application.Features.Cart.Commands;

public record RemoveFromCartCommand(Guid UserId, Guid ItemId) : IRequest<Result>;

public class RemoveFromCartCommandHandler : IRequestHandler<RemoveFromCartCommand, Result>
{
    private readonly StoreDbContext _dbContext;

    public RemoveFromCartCommandHandler(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(RemoveFromCartCommand request, CancellationToken cancellationToken)
    {
        var cart = await _dbContext.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == request.UserId, cancellationToken);
            
        if (cart == null) return Result.Failure(new Error("Cart.NotFound", "Cart not found"));

        cart.RemoveItem(request.ItemId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}
