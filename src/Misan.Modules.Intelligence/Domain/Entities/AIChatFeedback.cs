using Misan.Shared.Kernel.Abstractions;
using System;

namespace Misan.Modules.Intelligence.Domain.Entities;

public sealed class AIChatFeedback : Entity
{
    private AIChatFeedback(Guid id, Guid messageId, bool isPositive) : base(id)
    {
        MessageId = messageId;
        Type = isPositive ? "like" : "dislike";
        CreatedAt = DateTime.UtcNow;
    }

    private AIChatFeedback() { }

    public Guid MessageId { get; private set; }
    public string Type { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    public static AIChatFeedback Create(Guid messageId, bool isPositive)
    {
        return new AIChatFeedback(Guid.NewGuid(), messageId, isPositive);
    }
}
