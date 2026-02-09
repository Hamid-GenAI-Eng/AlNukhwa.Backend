using System;
using Misan.Modules.Identity.Domain.Enums;
using Misan.Shared.Kernel.Abstractions;

namespace Misan.Modules.Identity.Domain.Entities;

public sealed class User : Entity
{
    private User(
        Guid id,
        string email,
        string phone,
        string passwordHash,
        UserRole role,
        DateTime createdOnUtc)
        : base(id)
    {
        Email = email;
        Phone = phone;
        PasswordHash = passwordHash;
        Role = role;
        CreatedOnUtc = createdOnUtc;
    }

    private User() { } // For EF Core

    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public bool IsEmailVerified { get; private set; }
    public bool IsPhoneVerified { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime? UpdatedOnUtc { get; private set; }

    public static User Create(string email, string phone, string passwordHash, UserRole role)
    {
        return new User(Guid.NewGuid(), email, phone, passwordHash, role, DateTime.UtcNow);
    }

    public void VerifyEmail()
    {
        IsEmailVerified = true;
        UpdatedOnUtc = DateTime.UtcNow;
    }

    public void VerifyPhone()
    {
        IsPhoneVerified = true;
        UpdatedOnUtc = DateTime.UtcNow;
    }
    
    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        UpdatedOnUtc = DateTime.UtcNow;
    }
}
