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

namespace Misan.Modules.Profiles.Application.Features.Health.Commands;

public record UpdateHealthProfileCommand(
    Guid UserId,
    BodyType BodyType,
    BloodGroup BloodGroup) : IRequest<Result>;

public class UpdateHealthProfileValidator : AbstractValidator<UpdateHealthProfileCommand>
{
    public UpdateHealthProfileValidator()
    {
        RuleFor(x => x.BodyType).IsInEnum();
        RuleFor(x => x.BloodGroup).IsInEnum();
    }
}

public class UpdateHealthProfileCommandHandler : IRequestHandler<UpdateHealthProfileCommand, Result>
{
    private readonly ProfilesDbContext _dbContext;

    public UpdateHealthProfileCommandHandler(ProfilesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(UpdateHealthProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await _dbContext.Profiles
            .Include(p => p.HealthProfile)
            .FirstOrDefaultAsync(p => p.UserId == request.UserId, cancellationToken);
        
        if (profile is null) return Result.Failure(new Error("Profile.NotFound", "Create a profile first."));

        if (profile.HealthProfile is null)
        {
            // Create if missing (Robustness)
            var hp = HealthProfile.Create(profile.Id, request.BodyType, request.BloodGroup);
            _dbContext.HealthProfiles.Add(hp);
        }
        else
        {
            profile.HealthProfile.Update(request.BodyType, request.BloodGroup);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
