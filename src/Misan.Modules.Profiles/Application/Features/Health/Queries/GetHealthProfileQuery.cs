using MediatR;
using Microsoft.EntityFrameworkCore;
using Misan.Modules.Profiles.Domain.Entities;
using Misan.Modules.Profiles.Infrastructure.Database;
using Misan.Shared.Kernel.Abstractions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Profiles.Application.Features.Health.Queries;

public record GetHealthProfileQuery(Guid UserId) : IRequest<Result<HealthProfileResponse>>;

public record HealthProfileResponse(
    string BodyType,
    string BloodGroup,
    object[] Conditions,
    object[] Allergies,
    object[] Medications,
    object[] MedicalHistory);

public class GetHealthProfileQueryHandler : IRequestHandler<GetHealthProfileQuery, Result<HealthProfileResponse>>
{
    private readonly ProfilesDbContext _dbContext;

    public GetHealthProfileQueryHandler(ProfilesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<HealthProfileResponse>> Handle(GetHealthProfileQuery request, CancellationToken cancellationToken)
    {
        var profile = await _dbContext.Profiles
            .AsNoTracking()
            .Include(p => p.HealthProfile)
            .ThenInclude(hp => hp!.Conditions)
            .Include(p => p.HealthProfile)
            .ThenInclude(hp => hp!.Allergies)
            .Include(p => p.HealthProfile)
            .ThenInclude(hp => hp!.Medications)
            .Include(p => p.HealthProfile)
            .ThenInclude(hp => hp!.MedicalHistories)
            .FirstOrDefaultAsync(p => p.UserId == request.UserId, cancellationToken);

        if (profile is null || profile.HealthProfile is null)
        {
             return Result.Failure<HealthProfileResponse>(new Error("HealthProfile.NotFound", "Health Profile not found."));
        }

        var hp = profile.HealthProfile;

        return new HealthProfileResponse(
            hp.BodyType?.ToString() ?? "Unknown",
            hp.BloodGroup?.ToString() ?? "Unknown",
            hp.Conditions.Select(c => new { c.Id, c.Name, Severity = c.Severity.ToString(), c.DiagnosedDate }).ToArray(),
            hp.Allergies.Select(a => new { a.Id, a.Name, Severity = a.Severity.ToString(), a.Description }).ToArray(),
            hp.Medications.Select(m => new { m.Id, m.Name, m.Dosage, Type = m.Type.ToString(), m.ImageUrl, m.IsActive }).ToArray(),
            hp.MedicalHistories.Select(h => new { h.Id, h.Title, h.Description, h.Date, h.IsActive }).ToArray()
        );
    }
}
