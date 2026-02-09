using Microsoft.EntityFrameworkCore;
using Misan.Modules.Profiles.Domain.Entities;
using Misan.Modules.Profiles.Domain.Enums;

namespace Misan.Modules.Profiles.Infrastructure.Database;

public class ProfilesDbContext : DbContext
{
    public ProfilesDbContext(DbContextOptions<ProfilesDbContext> options) : base(options)
    {
    }

    public DbSet<Profile> Profiles { get; set; } = null!;
    public DbSet<HealthProfile> HealthProfiles { get; set; } = null!;
    public DbSet<Condition> Conditions { get; set; } = null!;
    public DbSet<Allergy> Allergies { get; set; } = null!;
    public DbSet<Medication> Medications { get; set; } = null!;
    public DbSet<MedicalHistory> MedicalHistories { get; set; } = null!;
    public DbSet<EmergencyContact> EmergencyContacts { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProfilesDbContext).Assembly);
        modelBuilder.HasDefaultSchema("profiles");

        // Seed Admin Profile (Matches Identity User: 99999999-...)
        var adminProfileId = Guid.Parse("99999999-9999-9999-9999-999999999998");
        var adminUserId = Guid.Parse("99999999-9999-9999-9999-999999999999");
        
        modelBuilder.Entity<Profile>().HasData(new 
        {
            Id = adminProfileId,
            UserId = adminUserId,
            FullName = "Misan Admin",
            Dob = new DateTime(1980, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Gender = Gender.Male, // 1
            City = "Lahore",
            MembershipTier = MembershipTier.Gold, // 3
            CreatedOnUtc = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        // Seed Admin HealthProfile
        modelBuilder.Entity<HealthProfile>().HasData(new
        {
            Id = Guid.Parse("99999999-9999-9999-9999-999999999997"),
            ProfileId = adminProfileId,
            BodyType = BodyType.Sanguine, // 1
            BloodGroup = BloodGroup.OPositive // ? Value 6
        });
    }
}
