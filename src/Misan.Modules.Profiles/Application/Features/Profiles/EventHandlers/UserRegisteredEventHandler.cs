using MediatR;
using Misan.Modules.Profiles.Domain.Entities;
using Misan.Modules.Profiles.Domain.Enums;
using Misan.Modules.Profiles.Infrastructure.Database;
using Misan.Shared.Kernel.IntegrationEvents;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Profiles.Application.Features.Profiles.EventHandlers;

public class UserRegisteredEventHandler : INotificationHandler<UserRegisteredEvent>
{
    private readonly ProfilesDbContext _dbContext;

    public UserRegisteredEventHandler(ProfilesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
    {
        // 1. Create Profile
        // Note: Providing default/empty values as these are not collected during initial registration.
        // User will update them later via "Update Profile" screen.
        var profile = Profile.Create(
            notification.UserId, 
            string.Empty, // FullName placeholder
            null,         // DOB
            Gender.Male,  // Default Gender (User must update)
            string.Empty  // City
        );

        _dbContext.Profiles.Add(profile);

        // 2. Create Health Profile
        // 1-to-1 relationship with Profile
        var healthProfile = HealthProfile.Create(
            profile.Id,
            null, // BodyType
            null  // BloodGroup
        );

        _dbContext.HealthProfiles.Add(healthProfile);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
