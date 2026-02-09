using MediatR;
using Microsoft.EntityFrameworkCore;
using Misan.Modules.Intelligence.Infrastructure.Database;
using Misan.Shared.Kernel.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Intelligence.Application.Features.AIChat.Queries;

public record GetAiSessionsQuery(Guid UserId) : IRequest<Result<List<AiSessionDto>>>;

public record AiSessionDto(Guid Id, string Title, DateTime UpdatedAt, bool TibbMode);

public class GetAiSessionsQueryHandler : IRequestHandler<GetAiSessionsQuery, Result<List<AiSessionDto>>>
{
    private readonly IntelligenceDbContext _dbContext;
    public GetAiSessionsQueryHandler(IntelligenceDbContext dbContext) => _dbContext = dbContext;

    public async Task<Result<List<AiSessionDto>>> Handle(GetAiSessionsQuery request, CancellationToken cancellationToken)
    {
        var sessions = await _dbContext.AIChatSessions
            .Where(s => s.UserId == request.UserId)
            .OrderByDescending(s => s.UpdatedAt)
            .Select(s => new AiSessionDto(s.Id, s.Title, s.UpdatedAt, s.TibbMode))
            .ToListAsync(cancellationToken);

        return Result.Success(sessions);
    }
}
