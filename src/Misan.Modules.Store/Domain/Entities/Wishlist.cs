using Misan.Shared.Kernel.Abstractions;
using System;

namespace Misan.Modules.Store.Domain.Entities;

public sealed class Wishlist : Entity
{
    private Wishlist(Guid id, Guid userId, string itemType, Guid itemId) : base(id)
    {
        UserId = userId;
        ItemType = itemType; // "Product", "Hakeem", "Article"
        ItemId = itemId;
        CreatedAt = DateTime.UtcNow;
    }

    private Wishlist() { }

    public Guid UserId { get; private set; }
    public string ItemType { get; private set; } = string.Empty;
    public Guid ItemId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static Wishlist Create(Guid userId, string itemType, Guid itemId)
    {
        return new Wishlist(Guid.NewGuid(), userId, itemType, itemId);
    }
}
