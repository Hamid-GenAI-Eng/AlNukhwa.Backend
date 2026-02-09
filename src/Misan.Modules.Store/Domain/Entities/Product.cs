using Misan.Shared.Kernel.Abstractions;
using Misan.Modules.Store.Domain.Enums;
using System;
using System.Collections.Generic;

namespace Misan.Modules.Store.Domain.Entities;

public sealed class Product : Entity
{
    private readonly List<string> _ingredients = new();
    
    private Product(Guid id, string name, string description, decimal price, MizajType mizaj, string category, string imageUrl, int stock) : base(id)
    {
        Name = name;
        Description = description;
        Price = price;
        Mizaj = mizaj;
        Category = category;
        ImageUrl = imageUrl;
        StockQty = stock;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    private Product() { }

    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public decimal? SalePrice { get; private set; }
    public MizajType Mizaj { get; private set; }
    public string Category { get; private set; } = string.Empty;
    public string ImageUrl { get; private set; } = string.Empty;
    public int StockQty { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    // Ratings are aggregated, or stored here? Let's store aggregate for performance.
    public double Rating { get; private set; }
    public int ReviewCount { get; private set; }

    public IReadOnlyCollection<string> Ingredients => _ingredients.AsReadOnly();

    public static Product Create(string name, string desc, decimal price, MizajType mizaj, string category, string image, int stock)
    {
        return new Product(Guid.NewGuid(), name, desc, price, mizaj, category, image, stock);
    }
    
    public void UpdateStock(int quantity)
    {
        StockQty = quantity;
    }

    public void UpdateDetails(string name, string desc, decimal price, MizajType mizaj, string category, string image)
    {
        Name = name;
        Description = desc;
        Price = price;
        Mizaj = mizaj;
        Category = category;
        ImageUrl = image;
    }

    public void AddIngredient(string ingredient)
    {
        _ingredients.Add(ingredient);
    }
}
