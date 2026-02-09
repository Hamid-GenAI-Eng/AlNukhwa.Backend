using System;
using Misan.Shared.Kernel.Abstractions;

namespace Misan.Modules.Identity.Domain.Entities;

public sealed class OTPToken : Entity
{
    private OTPToken(
        Guid id,
        Guid userId,
        string token,
        string type,
        DateTime expiresOnUtc)
        : base(id)
    {
        UserId = userId;
        Token = token;
        Type = type;
        ExpiresOnUtc = expiresOnUtc;
    }
    
    private OTPToken() { }

    public Guid UserId { get; private set; }
    public string Token { get; private set; }
    public string Type { get; private set; } // "Email" or "Phone"
    public DateTime ExpiresOnUtc { get; private set; }
    public DateTime? UsedOnUtc { get; private set; }

    public bool IsExpired => DateTime.UtcNow > ExpiresOnUtc;
    public bool IsUsed => UsedOnUtc.HasValue;

    public static OTPToken Create(Guid userId, string token, string type, TimeSpan expiry)
    {
        return new OTPToken(Guid.NewGuid(), userId, token, type, DateTime.UtcNow.Add(expiry));
    }

    public void MarkAsUsed()
    {
        if (IsUsed) throw new InvalidOperationException("Token already used.");
        UsedOnUtc = DateTime.UtcNow;
    }
}
