using MediatR;
using Microsoft.EntityFrameworkCore;
using Misan.Modules.Store.Domain.Entities;
using Misan.Modules.Store.Domain.Enums;
using Misan.Modules.Store.Infrastructure.Database;
using Misan.Shared.Kernel.Abstractions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Store.Application.Features.Orders.Commands;

public record CreateOrderCommand(Guid UserId, Guid ShippingAddressId, string PaymentMethod) : IRequest<Result<Guid>>;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<Guid>>
{
    private readonly StoreDbContext _dbContext;

    public CreateOrderCommandHandler(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // 1. Get Cart
        var cart = await _dbContext.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == request.UserId, cancellationToken);

        if (cart == null || !cart.Items.Any()) return Result.Failure<Guid>(new Error("Cart.Empty", "Cart is empty"));

        // 2. Validate Stock and Prices
        var productIds = cart.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _dbContext.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => p, cancellationToken);

        var order = Order.Create(request.UserId, request.ShippingAddressId, request.PaymentMethod);

        foreach (var item in cart.Items)
        {
            if (!products.TryGetValue(item.ProductId, out var product))
                return Result.Failure<Guid>(new Error("Product.NotFound", $"Product {item.ProductId} not found"));

            if (product.StockQty < item.Quantity)
                return Result.Failure<Guid>(new Error("Order.OutOfStock", $"Product {product.Name} is out of stock"));

            // Descrease stock? Or wait for payment? Let's decrease now (reservation) and release if cancelled.
            product.UpdateStock(product.StockQty - item.Quantity);
            
            var price = product.SalePrice ?? product.Price;
            order.AddItem(item.ProductId, product.Name, item.Quantity, price);
        }

        // 3. Save Order
        _dbContext.Orders.Add(order);
        
        // 4. Clear Cart
        cart.Clear();
        // Since we modified Cart Items (Removed) and Products (Stock) and added Order,
        // EF Core tracks all these changes.
        
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(order.Id);
    }
}
