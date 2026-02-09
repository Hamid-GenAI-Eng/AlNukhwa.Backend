using MediatR;
using Microsoft.EntityFrameworkCore;
using Misan.Modules.Store.Domain.Entities;
using Misan.Modules.Store.Domain.Enums;
using Misan.Modules.Store.Infrastructure.Database;
using Misan.Shared.Kernel.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Store.Application.Features.Products.Commands;

public record UpdateProductCommand(Guid Id, string Name, string Description, decimal Price, MizajType Mizaj, string Category, string ImageUrl, int Stock) : IRequest<Result>;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result>
{
    private readonly StoreDbContext _dbContext;

    public UpdateProductCommandHandler(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        // For partial updates, we might want separate commands or nullable fields.
        // Assuming full update for simplicity or mapping logic handled in API.
        // But since Entity properties are private setters, we need methods.
        // Oh wait, I didn't add Update methods to Product entity.
        
        var product = await _dbContext.Products.FindAsync(new object[] { request.Id }, cancellationToken);
        if (product == null) return Result.Failure(new Error("Product.NotFound", "Not found"));

        product.UpdateDetails(request.Name, request.Description, request.Price, request.Mizaj, request.Category, request.ImageUrl);
        product.UpdateStock(request.Stock);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return Result.Success(); 
    }
}
