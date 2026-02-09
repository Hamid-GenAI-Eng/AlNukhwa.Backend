using Misan.Shared.Kernel.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Misan.Modules.Store.Domain.Entities;

public sealed class Cart : Entity
{
    private readonly List<CartItem> _items = new();

    private Cart(Guid id, Guid userId) : base(id)
    {
        UserId = userId;
        UpdatedAt = DateTime.UtcNow;
    }

    private Cart() { }

    public Guid UserId { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

    public static Cart Create(Guid userId)
    {
        return new Cart(Guid.NewGuid(), userId);
    }

    public void AddItem(Guid productId, int quantity)
    {
        var existingParams = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existingParams != null)
        {
            existingParams.UpdateQuantity(existingParams.Quantity + quantity);
        }
        else
        {
            _items.Add(new CartItem(Guid.NewGuid(), Id, productId, quantity));
        }
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveItem(Guid itemId)
    {
        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            _items.Remove(item);
            UpdatedAt = DateTime.UtcNow;
        }
    }
    
    public void Clear()
    {
        _items.Clear();
        UpdatedAt = DateTime.UtcNow;
    }
}

public sealed class CartItem : Entity
{
    internal CartItem(Guid id, Guid cartId, Guid productId, int quantity) : base(id)
    {
        CartId = cartId;
        ProductId = productId;
        Quantity = quantity;
    }
    private CartItem() { }

    public Guid CartId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }

    public void UpdateQuantity(int quantity)
    {
        Quantity = quantity;
    }
}
