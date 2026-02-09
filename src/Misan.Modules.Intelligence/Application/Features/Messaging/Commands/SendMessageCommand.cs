using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Misan.Modules.Intelligence.Domain.Entities;
using Misan.Modules.Intelligence.Infrastructure.Database;
using Misan.Modules.Intelligence.Infrastructure.SignalR;
using Misan.Shared.Kernel.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Intelligence.Application.Features.Messaging.Commands;

public record SendMessageCommand(Guid UserId, Guid? ConversationId, Guid? RecipientId, string Content, string? AttachmentUrl) : IRequest<Result<Guid>>;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, Result<Guid>>
{
    private readonly IntelligenceDbContext _dbContext;
    private readonly IHubContext<ChatHub> _hubContext;

    public SendMessageCommandHandler(IntelligenceDbContext dbContext, IHubContext<ChatHub> hubContext)
    {
        _dbContext = dbContext;
        _hubContext = hubContext;
    }

    public async Task<Result<Guid>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        Conversation conversation;

        if (request.ConversationId.HasValue)
        {
            conversation = await _dbContext.Conversations.FindAsync(new object[] { request.ConversationId.Value }, cancellationToken);
            if (conversation == null) return Result.Failure<Guid>(new Error("Conversation.NotFound", "Conversation not found"));
        }
        else if (request.RecipientId.HasValue)
        {
            // Find existing conversation or create new
            // Simplified: Assuming direct 1-on-1 for now or creating new
            // Ideally check if conversation exists between these two users
            conversation = Conversation.Create(new List<Guid> { request.UserId, request.RecipientId.Value });
            _dbContext.Conversations.Add(conversation);
        }
        else
        {
            return Result.Failure<Guid>(new Error("Validation.Error", "RecipientId or ConversationId required"));
        }

        var message = Message.Create(conversation.Id, request.UserId, request.Content, request.AttachmentUrl);
        _dbContext.Messages.Add(message);
        conversation.UpdateTimestamp();

        await _dbContext.SaveChangesAsync(cancellationToken);

        // Broadcast to conversation group
        await _hubContext.Clients.Group(conversation.Id.ToString()).SendAsync("ReceiveMessage", new
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            SenderId = message.SenderId,
            Content = message.Content,
            SentAt = message.SentAt
        }, cancellationToken);

        return Result.Success(message.Id);
    }
}
