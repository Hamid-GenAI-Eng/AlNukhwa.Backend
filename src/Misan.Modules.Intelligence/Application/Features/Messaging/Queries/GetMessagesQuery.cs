using MediatR;
using Microsoft.EntityFrameworkCore;
using Misan.Modules.Intelligence.Infrastructure.Database;
using Misan.Shared.Kernel.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Intelligence.Application.Features.Messaging.Queries;

public record GetMessagesQuery(Guid ConversationId, Guid UserId) : IRequest<Result<List<MessageDto>>>;

public record MessageDto(Guid Id, Guid SenderId, string Content, DateTime SentAt, string? AttachmentUrl, bool IsRead);

public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, Result<List<MessageDto>>>
{
    private readonly IntelligenceDbContext _dbContext;
    public GetMessagesQueryHandler(IntelligenceDbContext dbContext) => _dbContext = dbContext;

    public async Task<Result<List<MessageDto>>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        // Verify participant
        var isParticipant = await _dbContext.ConversationParticipants
            .AnyAsync(p => p.ConversationId == request.ConversationId && p.UserId == request.UserId, cancellationToken);
        
        if (!isParticipant) return Result.Failure<List<MessageDto>>(new Error("Access.Denied", "User not in conversation"));

        var messages = await _dbContext.Messages
            .Where(m => m.ConversationId == request.ConversationId)
            .OrderBy(m => m.SentAt)
            .Select(m => new MessageDto(m.Id, m.SenderId, m.Content, m.SentAt, m.AttachmentUrl, m.ReadAt.HasValue))
            .ToListAsync(cancellationToken);

        return Result.Success(messages);
    }
}
