using Misan.Shared.Kernel.Abstractions;
using System;

namespace Misan.Modules.Intelligence.Domain.Entities;

public sealed class ConversationParticipant : Entity
{
    private ConversationParticipant(Guid id, Guid conversationId, Guid userId) : base(id)
    {
        ConversationId = conversationId;
        UserId = userId;
        JoinedAt = DateTime.UtcNow;
    }

    private ConversationParticipant() { }

    public Guid ConversationId { get; private set; }
    public Guid UserId { get; private set; }
    public Conversation Conversation { get; private set; } = null!;
    public DateTime JoinedAt { get; private set; }
    public DateTime? LastReadAt { get; private set; }

    public static ConversationParticipant Create(Guid conversationId, Guid userId)
    {
        return new ConversationParticipant(Guid.NewGuid(), conversationId, userId);
    }

    public void MarkAsRead() => LastReadAt = DateTime.UtcNow;
}
