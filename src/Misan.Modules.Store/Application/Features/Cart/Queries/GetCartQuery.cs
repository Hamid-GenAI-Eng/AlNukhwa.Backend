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

namespace Misan.Modules.Store.Application.Features.Cart.Queries;

public record CartItemDto(Guid Id, Guid ProductId, string ProductName, decimal Price, int Quantity, decimal Total);
public record CartDto(Guid Id, List<CartItemDto> Items, decimal SubTotal);

public record GetCartQuery(Guid UserId) : IRequest<Result<CartDto>>;

public class GetCartQueryHandler : IRequestHandler<GetCartQuery, Result<CartDto>>
{
    private readonly StoreDbContext _dbContext;

    public GetCartQueryHandler(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<CartDto>> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var cart = await _dbContext.Carts
            .Include(c => c.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.UserId == request.UserId, cancellationToken);

        if (cart == null)
        {
            return Result<CartDto>.Success(new CartDto(Guid.Empty, new List<CartItemDto>(), 0));
        }

        // We need Product details (Name, Price) which are not in CartItem (only ID).
        // Fetch products.
        var productIds = cart.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _dbContext.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => p, cancellationToken);

        var itemDtos = new List<CartItemDto>();
        decimal subTotal = 0;

        foreach (var item in cart.Items)
        {
            if (products.TryGetValue(item.ProductId, out var product))
            {
                var price = product.SalePrice ?? product.Price;
                var total = price * item.Quantity;
                subTotal += total;
                itemDtos.Add(new CartItemDto(item.Id, item.ProductId, product.Name, price, item.Quantity, total));
            }
        }

        return Result<CartDto>.Success(new CartDto(cart.Id, itemDtos, subTotal));
    }
}
