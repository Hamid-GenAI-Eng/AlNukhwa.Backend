using MediatR;
using Microsoft.EntityFrameworkCore;
using Misan.Modules.Store.Domain.Entities;
using Misan.Modules.Store.Domain.Enums;
using Misan.Modules.Store.Infrastructure.Database;
using Misan.Shared.Kernel.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Store.Application.Features.Products.Queries;

public record GetProductsQuery(string? Category, MizajType? Mizaj, string? Search) : IRequest<Result<List<Product>>>;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, Result<List<Product>>>
{
    private readonly StoreDbContext _dbContext;

    public GetProductsQueryHandler(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<List<Product>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Category))
        {
            query = query.Where(p => p.Category == request.Category);
        }

        if (request.Mizaj.HasValue)
        {
            query = query.Where(p => p.Mizaj == request.Mizaj.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(p => p.Name.Contains(request.Search) || p.Description.Contains(request.Search));
        }

        var products = await query.OrderByDescending(p => p.CreatedAt).ToListAsync(cancellationToken);

        return Result<List<Product>>.Success(products);
    }
}
