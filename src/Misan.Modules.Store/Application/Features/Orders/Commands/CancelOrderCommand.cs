using MediatR;
using Misan.Modules.Store.Infrastructure.Database;
using Misan.Shared.Kernel.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Store.Application.Features.Orders.Commands;

public record CancelOrderCommand(Guid OrderId, Guid UserId) : IRequest<Result>;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Result>
{
    private readonly StoreDbContext _dbContext;

    public CancelOrderCommandHandler(StoreDbContext dbContext) => _dbContext = dbContext;

    public async Task<Result> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders.FindAsync(new object[] { request.OrderId }, cancellationToken);
        if (order == null || order.UserId != request.UserId) 
            return Result.Failure(new Error("Order.NotFound", "Order not found"));

        // Simplistic state check
        if (order.Status == Domain.Enums.OrderStatus.Shipped || order.Status == Domain.Enums.OrderStatus.Delivered)
            return Result.Failure(new Error("Order.CannotCancel", "Cannot cancel shipped/delivered order"));

        // TODO: Ideally verify this status transition
        // For now, we don't have a public setter for Status on Order entity except specific methods.
        // I'll need to add a Cancel method to Order entity or use a hack for now. 
        // Let's add Cancel method to Order entity via partial or just reflection? No, Entity modification is best.
        
        order.Cancel();
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return Result.Success(); 
    }
}
