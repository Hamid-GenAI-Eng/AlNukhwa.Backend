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

public record GetConversationsQuery(Guid UserId) : IRequest<Result<List<ConversationDto>>>;

public record ConversationDto(Guid Id, DateTime UpdatedAt, string LastMessage, int UnreadCount);

public class GetConversationsQueryHandler : IRequestHandler<GetConversationsQuery, Result<List<ConversationDto>>>
{
    private readonly IntelligenceDbContext _dbContext;
    public GetConversationsQueryHandler(IntelligenceDbContext dbContext) => _dbContext = dbContext;

    public async Task<Result<List<ConversationDto>>> Handle(GetConversationsQuery request, CancellationToken cancellationToken)
    {
        var dtos = await _dbContext.ConversationParticipants
            .Where(p => p.UserId == request.UserId)
            .Select(p => new
            {
                p.Conversation.Id,
                p.Conversation.UpdatedAt,
                LastMessage = p.Conversation.Messages.OrderByDescending(m => m.SentAt).Select(m => m.Content).FirstOrDefault(),
                UnreadCount = p.Conversation.Messages.Count(m => m.SentAt > (p.LastReadAt ?? DateTime.MinValue))
            })
            .OrderByDescending(x => x.UpdatedAt)
            .ToListAsync(cancellationToken);

        var result = dtos.Select(x => new ConversationDto(x.Id, x.UpdatedAt, x.LastMessage ?? "", x.UnreadCount)).ToList();
        return Result.Success(result);
    }
}
