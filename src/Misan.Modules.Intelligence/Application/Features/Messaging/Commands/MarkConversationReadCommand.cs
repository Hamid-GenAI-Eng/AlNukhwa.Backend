using MediatR;
using Microsoft.EntityFrameworkCore;
using Misan.Modules.Intelligence.Infrastructure.Database;
using Misan.Shared.Kernel.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Intelligence.Application.Features.Messaging.Commands;

public record MarkConversationReadCommand(Guid ConversationId, Guid UserId) : IRequest<Result>;

public class MarkConversationReadCommandHandler : IRequestHandler<MarkConversationReadCommand, Result>
{
    private readonly IntelligenceDbContext _dbContext;
    public MarkConversationReadCommandHandler(IntelligenceDbContext dbContext) => _dbContext = dbContext;

    public async Task<Result> Handle(MarkConversationReadCommand request, CancellationToken cancellationToken)
    {
        var participant = await _dbContext.ConversationParticipants
            .FirstOrDefaultAsync(p => p.ConversationId == request.ConversationId && p.UserId == request.UserId, cancellationToken);

        if (participant == null) return Result.Failure(new Error("Conversation.NotFound", "User not in conversation"));

        participant.MarkAsRead();
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
