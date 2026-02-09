using System;
using Misan.Modules.Profiles.Domain.Enums;
using Misan.Shared.Kernel.Abstractions;

namespace Misan.Modules.Profiles.Domain.Entities;

public sealed class Profile : Entity
{
    private Profile(
        Guid id,
        Guid userId,
        string fullName,
        DateTime? dob,
        Gender gender,
        string city)
        : base(id)
    {
        UserId = userId;
        FullName = fullName;
        Dob = dob;
        Gender = gender;
        City = city;
        MembershipTier = MembershipTier.Bronze;
        CreatedOnUtc = DateTime.UtcNow;
    }

    private Profile() { }

    public Guid UserId { get; private set; } // FK to Identity User (Logical)
    public string FullName { get; private set; } = string.Empty;
    public string? AvatarUrl { get; private set; }
    public DateTime? Dob { get; private set; }
    public Gender Gender { get; private set; }
    public string City { get; private set; } = string.Empty;
    public MembershipTier MembershipTier { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime? UpdatedOnUtc { get; private set; }
    
    // Navigation property for HealthProfile (1-to-1)
    public HealthProfile? HealthProfile { get; private set; }

    public static Profile Create(Guid userId, string fullName, DateTime? dob, Gender gender, string city)
    {
        return new Profile(Guid.NewGuid(), userId, fullName, dob, gender, city);
    }

    public void UpdateDetails(string fullName, DateTime? dob, Gender gender, string city)
    {
        FullName = fullName;
        Dob = dob;
        Gender = gender;
        City = city;
        UpdatedOnUtc = DateTime.UtcNow;
    }

    public void SetAvatar(string avatarUrl)
    {
        AvatarUrl = avatarUrl;
        UpdatedOnUtc = DateTime.UtcNow;
    }

    public void UpgradeMembership(MembershipTier tier)
    {
        MembershipTier = tier;
        UpdatedOnUtc = DateTime.UtcNow;
    }
}
