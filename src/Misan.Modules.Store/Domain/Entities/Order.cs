using Misan.Shared.Kernel.Abstractions;
using Misan.Modules.Store.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Misan.Modules.Store.Domain.Entities;

public sealed class Order : Entity
{
    private readonly List<OrderItem> _items = new();

    private Order(Guid id, Guid userId, Guid shippingAddressId, string paymentMethod) : base(id)
    {
        UserId = userId;
        ShippingAddressId = shippingAddressId;
        PaymentMethod = paymentMethod;
        Status = OrderStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    private Order() { }

    public Guid UserId { get; private set; }
    public Guid ShippingAddressId { get; private set; }
    public OrderStatus Status { get; private set; }
    public string PaymentMethod { get; private set; } = string.Empty;
    public DateTime? PaidAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    public decimal SubTotal { get; private set; }
    public decimal ShippingCost { get; private set; }
    public decimal Tax { get; private set; }
    public decimal Total { get; private set; }
    
    public string? TrackingNumber { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public static Order Create(Guid userId, Guid addressId, string paymentMethod)
    {
        return new Order(Guid.NewGuid(), userId, addressId, paymentMethod);
    }

    public void AddItem(Guid productId, string productName, int quantity, decimal price)
    {
        _items.Add(new OrderItem(Guid.NewGuid(), Id, productId, productName, quantity, price));
        CalculateTotals();
    }
    
    public void CalculateTotals()
    {
        SubTotal = _items.Sum(i => i.Price * i.Quantity);
        // Simplified tax/shipping logic
        Tax = SubTotal * 0.05m; 
        ShippingCost = 200; // Flat rate for now
        Total = SubTotal + Tax + ShippingCost;
    }

    public void MarkAsPaid()
    {
        Status = OrderStatus.Processing;
        PaidAt = DateTime.UtcNow;
    }
    
    public void Ship(string trackingNumber)
    {
        Status = OrderStatus.Shipped;
        TrackingNumber = trackingNumber;
    }

    public void Cancel()
    {
        Status = OrderStatus.Cancelled;
    }
}

public sealed class OrderItem : Entity
{
    internal OrderItem(Guid id, Guid orderId, Guid productId, string productName, int quantity, decimal price) : base(id)
    {
        OrderId = orderId;
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        Price = price;
    }
    private OrderItem() { }

    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }
}
