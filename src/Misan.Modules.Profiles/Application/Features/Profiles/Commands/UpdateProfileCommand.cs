using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Misan.Modules.Profiles.Domain.Entities;
using Misan.Modules.Profiles.Domain.Enums;
using Misan.Modules.Profiles.Infrastructure.Database;
using Misan.Shared.Kernel.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Profiles.Application.Features.Profiles.Commands;

public record UpdateProfileCommand(
    Guid UserId,
    string FullName,
    DateTime? Dob,
    Gender Gender, // Changed from string to Gender
    string City) : IRequest<Result>;

public class UpdateProfileValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Gender).IsInEnum();
    }
}

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Result>
{
    private readonly ProfilesDbContext _dbContext;

    public UpdateProfileCommandHandler(ProfilesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        try 
        {
            var profile = await _dbContext.Profiles
                .FirstOrDefaultAsync(p => p.UserId == request.UserId, cancellationToken);
            
            // Fix: Ensure DOB is UTC to prevent Npgsql errors
            DateTime? dobUtc = request.Dob.HasValue 
                ? DateTime.SpecifyKind(request.Dob.Value, DateTimeKind.Utc) 
                : null;

            if (profile is null)
            {
                 // Fallback create logic (mirrors Event Handler)
                 Serilog.Log.Warning("Profile not found for user {UserId}. Creating fallback profile.", request.UserId);
                 
                 profile = Profile.Create(request.UserId, request.FullName, dobUtc, request.Gender, request.City);
                 _dbContext.Profiles.Add(profile);

                 // Also create HealthProfile
                 var healthProfile = HealthProfile.Create(profile.Id, null, null);
                 _dbContext.HealthProfiles.Add(healthProfile);
            }
            else
            {
                 profile.UpdateDetails(request.FullName, dobUtc, request.Gender, request.City);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "Error updating profile for User {UserId}", request.UserId);
            // Re-throw to let GlobalExceptionHandler return 500, but now we have logged it cleanly.
            throw; 
        }
    }
}
