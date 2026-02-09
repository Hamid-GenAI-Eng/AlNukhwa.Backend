using MediatR;
using Microsoft.EntityFrameworkCore;
using Misan.Modules.Practitioner.Domain.Entities;
using Misan.Modules.Practitioner.Domain.Enums;
using Misan.Modules.Practitioner.Infrastructure.Database;
using Misan.Shared.Kernel.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Practitioner.Application.Features.Practitioner.Queries;

public record HakeemResponse(
    Guid Id, 
    Guid UserId, 
    string FullName, 
    string AvatarUrl, 
    string City, 
    decimal Rating, 
    bool Verified, 
    int YearsExperience,
    List<string> Specializations,
    double Score, // For debug/algorithm transparency
    bool IsNewcomer
);

public record GetHakeemsQuery(
    string? SearchTerm, 
    string? City, 
    string? Specialization, 
    int Page = 1, 
    int PageSize = 10) : IRequest<Result<List<HakeemResponse>>>;

public class GetHakeemsQueryHandler : IRequestHandler<GetHakeemsQuery, Result<List<HakeemResponse>>>
{
    private readonly PractitionerDbContext _dbContext;

    public GetHakeemsQueryHandler(PractitionerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<List<HakeemResponse>>> Handle(GetHakeemsQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Hakeems
            .Include(h => h.Specializations)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
        }

        if (!string.IsNullOrEmpty(request.Specialization))
        {
            query = query.Where(h => h.Specializations.Any(s => s.SpecializationName.Contains(request.Specialization)));
        }

        var hakeems = await query.ToListAsync(cancellationToken);

        var responses = new List<HakeemResponse>();
        var random = new Random();

        foreach (var h in hakeems)
        {
            double activityScore = random.NextDouble() * 100; 
            bool isNewcomer = h.VerifiedAt.HasValue && (DateTime.UtcNow - h.VerifiedAt.Value).TotalDays < 30;
            double newcomerScore = isNewcomer ? 100 : 0;
            double ratingScore = (double)h.Rating * 20;
            double randomScore = random.NextDouble() * 100;
            double totalScore = (ratingScore * 0.4) + (activityScore * 0.3) + (newcomerScore * 0.2) + (randomScore * 0.1);

            responses.Add(new HakeemResponse(
                h.Id,
                h.UserId,
                "Name Placeholder",
                "Avatar Placeholder",
                "City Placeholder", 
                h.Rating,
                h.VerificationStatus == VerificationStatus.Verified,
                h.YearsOfExperience,
                h.Specializations.Select(s => s.SpecializationName).ToList(),
                totalScore,
                isNewcomer
            ));
        }

        var sortedResponse = responses.OrderByDescending(x => x.Score)
                                      .Skip((request.Page - 1) * request.PageSize)
                                      .Take(request.PageSize)
                                      .ToList();

        return Result.Success(sortedResponse);
    }
}
