using Misan.Shared.Kernel.Abstractions;
using System;
using System.Collections.Generic;

namespace Misan.Modules.Intelligence.Domain.Entities;

public sealed class AIChatMessage : Entity
{
    private readonly List<AIChatFeedback> _feedback = new();

    private AIChatMessage(Guid id, Guid sessionId, string role, string content, string? imageUrl) : base(id)
    {
        SessionId = sessionId;
        Role = role; // "user" or "assistant"
        Content = content;
        ImageUrl = imageUrl;
        CreatedAt = DateTime.UtcNow;
    }

    private AIChatMessage() { }

    public Guid SessionId { get; private set; }
    public string Role { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public string? ImageUrl { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public IReadOnlyCollection<AIChatFeedback> Feedback => _feedback.AsReadOnly();

    public static AIChatMessage Create(Guid sessionId, string role, string content, string? imageUrl = null)
    {
        return new AIChatMessage(Guid.NewGuid(), sessionId, role, content, imageUrl);
    }

    public void AddFeedback(bool isPositive)
    {
        _feedback.Add(AIChatFeedback.Create(Id, isPositive));
    }
}
