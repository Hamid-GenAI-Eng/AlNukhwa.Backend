using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Misan.Modules.Profiles.Domain.Entities;
using Misan.Modules.Profiles.Domain.Enums;

namespace Misan.Modules.Profiles.Infrastructure.Database.Configurations;

public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => p.UserId).IsUnique(); // One profile per user
        builder.Property(p => p.FullName).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Gender).HasConversion<string>();
        builder.Property(p => p.MembershipTier).HasConversion<string>();

        builder.HasOne(p => p.HealthProfile)
            .WithOne()
            .HasForeignKey<HealthProfile>(hp => hp.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class HealthProfileConfiguration : IEntityTypeConfiguration<HealthProfile>
{
    public void Configure(EntityTypeBuilder<HealthProfile> builder)
    {
        builder.HasKey(hp => hp.Id);
        builder.Property(hp => hp.BodyType).HasConversion<string>();
        builder.Property(hp => hp.BloodGroup).HasConversion<string>();

        builder.HasMany(hp => hp.Conditions).WithOne().HasForeignKey(c => c.HealthProfileId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(hp => hp.Allergies).WithOne().HasForeignKey(a => a.HealthProfileId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(hp => hp.Medications).WithOne().HasForeignKey(m => m.HealthProfileId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(hp => hp.MedicalHistories).WithOne().HasForeignKey(bh => bh.HealthProfileId).OnDelete(DeleteBehavior.Cascade);
    }
}
