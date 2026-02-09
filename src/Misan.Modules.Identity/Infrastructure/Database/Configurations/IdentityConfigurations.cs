using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Misan.Modules.Identity.Domain.Entities;
using Misan.Modules.Identity.Domain.Enums;

namespace Misan.Modules.Identity.Infrastructure.Database.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(255);
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.Phone).HasMaxLength(50);
        builder.HasIndex(u => u.Phone).IsUnique(); // Optional: if phone uniqueness is required
        builder.Property(u => u.Role).HasConversion<string>();
    }
}

public class OTPTokenConfiguration : IEntityTypeConfiguration<OTPToken>
{
    public void Configure(EntityTypeBuilder<OTPToken> builder)
    {
        builder.HasKey(t => t.Id);
        builder.HasIndex(t => t.Token);
        builder.Property(t => t.Type).HasMaxLength(20);
    }
}

public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.HasKey(s => s.Id);
        builder.HasIndex(s => s.RefreshToken);
        builder.Property(s => s.RefreshToken).IsRequired().HasMaxLength(500);
    }
}
