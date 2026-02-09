using MediatR;
using Microsoft.EntityFrameworkCore;
using Misan.Modules.Store.Domain.Entities;
using Misan.Modules.Store.Domain.Enums;
using Misan.Modules.Store.Infrastructure.Database;
using Misan.Shared.Kernel.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Store.Application.Features.Orders.Queries;

// Get Order Detail
public record GetOrderQuery(Guid OrderId, Guid UserId) : IRequest<Result<Order>>;

public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, Result<Order>>
{
    private readonly StoreDbContext _dbContext;
    public GetOrderQueryHandler(StoreDbContext dbContext) => _dbContext = dbContext;

    public async Task<Result<Order>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId && o.UserId == request.UserId, cancellationToken);
        
        return order != null ? Result.Success(order) : Result.Failure<Order>(new Error("Order.NotFound", "Order not found"));
    }
}

// Get Invoice
public record GetOrderInvoiceQuery(Guid OrderId, Guid UserId) : IRequest<Result<string>>;
public class GetOrderInvoiceQueryHandler : IRequestHandler<GetOrderInvoiceQuery, Result<string>>
{
    public Task<Result<string>> Handle(GetOrderInvoiceQuery request, CancellationToken cancellationToken)
    {
        // Mock PDF generation
        return Task.FromResult(Result.Success($"https://misan-platform.com/invoices/{request.OrderId}.pdf"));
    }
}

// Track Shipment
public record GetOrderTrackingQuery(Guid OrderId, Guid UserId) : IRequest<Result<string>>;
public class GetOrderTrackingQueryHandler : IRequestHandler<GetOrderTrackingQuery, Result<string>>
{
    public Task<Result<string>> Handle(GetOrderTrackingQuery request, CancellationToken cancellationToken)
    {
        // Mock Tracking
        return Task.FromResult(Result.Success("TCS-TRACKING-123456789"));
    }
}
