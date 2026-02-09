using MediatR;
using Microsoft.EntityFrameworkCore;
using Misan.Modules.Profiles.Domain.Entities;
using Misan.Modules.Profiles.Infrastructure.Database;
using Misan.Shared.Kernel.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Profiles.Application.Features.Profiles.Queries;

public record GetProfileQuery(Guid UserId) : IRequest<Result<ProfileResponse>>;

public record ProfileResponse(
    Guid Id, 
    string FullName, 
    string AvatarUrl, 
    DateTime? Dob, 
    string Gender, 
    string City, 
    string MembershipTier);

public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, Result<ProfileResponse>>
{
    private readonly ProfilesDbContext _dbContext;

    public GetProfileQueryHandler(ProfilesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<ProfileResponse>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var profile = await _dbContext.Profiles
            .FirstOrDefaultAsync(p => p.UserId == request.UserId, cancellationToken);
        
        if (profile is null)
        {
            return Result.Failure<ProfileResponse>(new Error("Profile.NotFound", "Profile not found."));
        }

        return new ProfileResponse(
            profile.Id,
            profile.FullName,
            profile.AvatarUrl ?? "",
            profile.Dob,
            profile.Gender.ToString(),
            profile.City,
            profile.MembershipTier.ToString());
    }
}
