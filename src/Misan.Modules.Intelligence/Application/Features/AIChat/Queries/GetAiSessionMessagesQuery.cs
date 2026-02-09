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

public record GetAiSessionMessagesQuery(Guid SessionId, Guid UserId) : IRequest<Result<List<AiMessageDto>>>;

public record AiMessageDto(Guid Id, string Role, string Content, DateTime CreatedAt, string? ImageUrl);

public class GetAiSessionMessagesQueryHandler : IRequestHandler<GetAiSessionMessagesQuery, Result<List<AiMessageDto>>>
{
    private readonly IntelligenceDbContext _dbContext;
    public GetAiSessionMessagesQueryHandler(IntelligenceDbContext dbContext) => _dbContext = dbContext;

    public async Task<Result<List<AiMessageDto>>> Handle(GetAiSessionMessagesQuery request, CancellationToken cancellationToken)
    {
        var session = await _dbContext.AIChatSessions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.SessionId, cancellationToken);

        if (session == null || session.UserId != request.UserId)
            return Result.Failure<List<AiMessageDto>>(new Error("Session.NotFound", "Session not found"));

        var messages = await _dbContext.AIChatMessages
            .Where(m => m.SessionId == request.SessionId)
            .OrderBy(m => m.CreatedAt)
            .Select(m => new AiMessageDto(m.Id, m.Role, m.Content, m.CreatedAt, m.ImageUrl))
            .ToListAsync(cancellationToken);

        return Result.Success(messages);
    }
}
