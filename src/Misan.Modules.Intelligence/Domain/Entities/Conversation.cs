using Misan.Shared.Kernel.Abstractions;
using System;
using System.Collections.Generic;

namespace Misan.Modules.Intelligence.Domain.Entities;

public sealed class Conversation : Entity
{
    private readonly List<ConversationParticipant> _participants = new();
    private readonly List<Message> _messages = new();

    private Conversation(Guid id) : base(id) 
    {
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    private Conversation() { } 

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public IReadOnlyCollection<ConversationParticipant> Participants => _participants.AsReadOnly();
    public IReadOnlyCollection<Message> Messages => _messages.AsReadOnly();

    public static Conversation Create(List<Guid> participantIds)
    {
        var conversation = new Conversation(Guid.NewGuid());
        foreach (var userId in participantIds)
        {
            conversation._participants.Add(ConversationParticipant.Create(conversation.Id, userId));
        }
        return conversation;
    }

    public void UpdateTimestamp() => UpdatedAt = DateTime.UtcNow;
}
