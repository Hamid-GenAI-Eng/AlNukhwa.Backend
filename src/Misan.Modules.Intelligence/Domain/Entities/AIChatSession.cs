using Misan.Shared.Kernel.Abstractions;
using System;
using System.Collections.Generic;

namespace Misan.Modules.Intelligence.Domain.Entities;

public sealed class AIChatSession : Entity
{
    private readonly List<AIChatMessage> _messages = new();

    private AIChatSession(Guid id, Guid userId, string title, bool tibbMode) : base(id)
    {
        UserId = userId;
        Title = title;
        TibbMode = tibbMode;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    private AIChatSession() { }

    public Guid UserId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public bool TibbMode { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public IReadOnlyCollection<AIChatMessage> Messages => _messages.AsReadOnly();

    public static AIChatSession Create(Guid userId, string title = "New Chat", bool tibbMode = false)
    {
        return new AIChatSession(Guid.NewGuid(), userId, title, tibbMode);
    }

    public void AddMessage(AIChatMessage message)
    {
        _messages.Add(message);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePreferences(bool tibbMode)
    {
        TibbMode = tibbMode;
        UpdatedAt = DateTime.UtcNow;
    }
}
