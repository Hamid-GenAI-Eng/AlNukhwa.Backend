using Misan.Shared.Kernel.Abstractions;
using System;

namespace Misan.Modules.Intelligence.Domain.Entities;

public sealed class Message : Entity
{
    private Message(Guid id, Guid conversationId, Guid senderId, string content, string? attachmentUrl) : base(id)
    {
        ConversationId = conversationId;
        SenderId = senderId;
        Content = content;
        AttachmentUrl = attachmentUrl;
        SentAt = DateTime.UtcNow;
    }

    private Message() { }

    public Guid ConversationId { get; private set; }
    public Guid SenderId { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public string? AttachmentUrl { get; private set; }
    public DateTime SentAt { get; private set; }
    public DateTime? ReadAt { get; private set; }

    public static Message Create(Guid conversationId, Guid senderId, string content, string? attachmentUrl = null)
    {
        return new Message(Guid.NewGuid(), conversationId, senderId, content, attachmentUrl);
    }

    public void MarkAsRead()
    {
        if (!ReadAt.HasValue) ReadAt = DateTime.UtcNow;
    }
}
