using Microsoft.EntityFrameworkCore;
using Misan.Modules.Store.Domain.Entities;

namespace Misan.Modules.Store.Infrastructure.Database;

public class StoreDbContext : DbContext
{
    public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Wishlist> Wishlists { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Payout> Payouts { get; set; }
    public DbSet<ProductReview> ProductReviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("store");

        modelBuilder.Entity<Product>(b => {
            b.HasKey(p => p.Id);
            b.Property(p => p.Price).HasPrecision(18, 2);
            b.Property(p => p.SalePrice).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Order>(b => {
             b.HasKey(o => o.Id);
             b.HasMany(o => o.Items).WithOne().HasForeignKey(i => i.OrderId).OnDelete(DeleteBehavior.Cascade);
             b.Property(o => o.SubTotal).HasPrecision(18, 2);
             b.Property(o => o.Tax).HasPrecision(18, 2);
             b.Property(o => o.ShippingCost).HasPrecision(18, 2);
             b.Property(o => o.Total).HasPrecision(18, 2);
        });

        modelBuilder.Entity<OrderItem>(b => {
            b.HasKey(o => o.Id);
            b.Property(o => o.Price).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Cart>(b => {
            b.HasKey(c => c.Id);
            b.HasMany(c => c.Items).WithOne().HasForeignKey(i => i.CartId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Transaction>(b => {
            b.HasKey(t => t.Id);
            b.Property(t => t.GrossAmount).HasPrecision(18, 2);
            b.Property(t => t.PlatformFee).HasPrecision(18, 2);
            b.Property(t => t.NetAmount).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Payout>(b => {
            b.HasKey(p => p.Id);
            b.Property(p => p.Amount).HasPrecision(18, 2);
        });

        modelBuilder.Entity<ProductReview>(b => {
             b.HasKey(r => r.Id);
             b.HasOne<Product>().WithMany().HasForeignKey(r => r.ProductId).OnDelete(DeleteBehavior.Cascade);
        });

        // Seed
        modelBuilder.Entity<Product>().HasData(
            new 
            { 
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), 
                Name = "Seed Honey", 
                Description = "Pure Honey", 
                Price = 50m, 
                Mizaj = Domain.Enums.MizajType.HotWet, 
                Category = "Food", 
                ImageUrl = "http://example.com/honey.png", 
                StockQty = 100,
                IsActive = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Rating = 0.0,
                ReviewCount = 0
            }
        );
    }
}
