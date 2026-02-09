using MediatR;
using Microsoft.EntityFrameworkCore;
using DomainCart = Misan.Modules.Store.Domain.Entities.Cart;
using Misan.Modules.Store.Infrastructure.Database;
using Misan.Shared.Kernel.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Store.Application.Features.Cart.Commands;

public record AddToCartCommand(Guid UserId, Guid ProductId, int Quantity) : IRequest<Result>;

public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, Result>
{
    private readonly StoreDbContext _dbContext;

    public AddToCartCommandHandler(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products.FindAsync(new object[] { request.ProductId }, cancellationToken);
        if (product == null) return Result.Failure(new Error("Product.NotFound", "Not found"));

        if (product.StockQty < request.Quantity) return Result.Failure(new Error("Cart.OutOfStock", "Not enough stock"));

        var cart = await _dbContext.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == request.UserId, cancellationToken);

        if (cart == null)
        {
            cart = DomainCart.Create(request.UserId);
            _dbContext.Carts.Add(cart);
        }

        cart.AddItem(request.ProductId, request.Quantity);

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
