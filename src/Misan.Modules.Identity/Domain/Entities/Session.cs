using System;
using Misan.Shared.Kernel.Abstractions;

namespace Misan.Modules.Identity.Domain.Entities;

public sealed class Session : Entity
{
    private Session(
        Guid id,
        Guid userId,
        string refreshToken,
        string deviceInfo,
        DateTime createdOnUtc,
        DateTime expiresOnUtc)
        : base(id)
    {
        UserId = userId;
        RefreshToken = refreshToken;
        DeviceInfo = deviceInfo;
        CreatedOnUtc = createdOnUtc;
        ExpiresOnUtc = expiresOnUtc;
    }

    private Session() { }

    public Guid UserId { get; private set; }
    public string RefreshToken { get; private set; }
    public string DeviceInfo { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime ExpiresOnUtc { get; private set; }
    public bool IsRevoked { get; private set; }

    public static Session Create(Guid userId, string refreshToken, string deviceInfo, DateTime expiresOnUtc)
    {
        return new Session(Guid.NewGuid(), userId, refreshToken, deviceInfo, DateTime.UtcNow, expiresOnUtc);
    }

    public void Revoke()
    {
        IsRevoked = true;
    }
}
