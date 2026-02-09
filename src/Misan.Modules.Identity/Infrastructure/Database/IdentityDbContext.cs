using Microsoft.EntityFrameworkCore;
using Misan.Modules.Identity.Domain.Entities;
using Misan.Modules.Identity.Domain.Enums;

namespace Misan.Modules.Identity.Infrastructure.Database;

public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<OTPToken> OTPTokens { get; set; } = null!;
    public DbSet<Session> Sessions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
        modelBuilder.HasDefaultSchema("identity");

        // Seed Admin User
        modelBuilder.Entity<User>().HasData(new 
        {
            Id = Guid.Parse("99999999-9999-9999-9999-999999999999"),
            Email = "MisanAdmin@codeenvision.com",
            Phone = "+920000000000",
            PasswordHash = "$2a$11$wnzGoYEsJMQfVA5Fqn2GqOgA4DfgZ.hqCQx3jfnu5oTgnfgYIOxta",
            Role = UserRole.Admin,
            IsEmailVerified = true,
            IsPhoneVerified = true,
            CreatedOnUtc = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}
