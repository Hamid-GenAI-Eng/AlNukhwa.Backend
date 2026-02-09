using MediatR;
using Misan.Modules.Store.Domain.Entities;
using Misan.Modules.Store.Domain.Enums;
using Misan.Modules.Store.Infrastructure.Database;
using Misan.Shared.Kernel.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Store.Application.Features.Products.Commands;

public record CreateProductCommand(string Name, string Description, decimal Price, MizajType Mizaj, string Category, string ImageUrl, int Stock, string[] Ingredients) : IRequest<Result<Guid>>;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    private readonly StoreDbContext _dbContext;

    public CreateProductCommandHandler(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = Product.Create(request.Name, request.Description, request.Price, request.Mizaj, request.Category, request.ImageUrl, request.Stock);
        
        foreach (var ing in request.Ingredients)
        {
            product.AddIngredient(ing);
        }

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(product.Id);
    }
}
