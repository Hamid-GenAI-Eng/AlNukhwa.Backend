using MediatR;
using Misan.Modules.Practitioner.Infrastructure.Database;
using Misan.Shared.Kernel.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Practitioner.Application.Features.Practitioner.Queries;

public record GetHakeemDashboardQuery(Guid UserId) : IRequest<Result<HakeemDashboardDto>>;

public record HakeemDashboardDto(
    int TotalPatients,
    decimal TotalEarnings,
    int UpcomingAppointments,
    decimal Rating
);

public class GetHakeemDashboardQueryHandler : IRequestHandler<GetHakeemDashboardQuery, Result<HakeemDashboardDto>>
{
    private readonly PractitionerDbContext _context;

    public GetHakeemDashboardQueryHandler(PractitionerDbContext context)
    {
        _context = context;
    }

    public async Task<Result<HakeemDashboardDto>> Handle(GetHakeemDashboardQuery request, CancellationToken cancellationToken)
    {
        var hakeem = await _context.Hakeems
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.UserId == request.UserId, cancellationToken);

        if (hakeem == null)
            return Result.Failure<HakeemDashboardDto>(new Error("Hakeem.NotFound", "Hakeem profile not found."));

        // Note: Cross-module data (Patients, Appointments) is ideally fetched via 
        // 1. Integration Events (replicating data) 
        // 2. Direct API calls (internal HTTP)
        // 3. Shared Database Views (Modular Monolith shortcut).
        
        // For this task, we will query local context data if available, or placeholder/mock 
        // for data that strictly belongs to 'Clinical' module (Appointments).
        
        // In Modular Monolith, often Read-Models are used. 
        // Since we don't have Clinical DbContext here, we can't query Appointments directly 
        // unless we have a 'Read Model' in Practitioner schema synced via events.
        
        // TEMPORARY: Return 0 or Simulated Data until Event Sync is fully implemented.
        // However, 'Rating' IS on Hakeem entity.
        
        var dashboard = new HakeemDashboardDto(
            TotalPatients: 0, // Needs syncing from Clinical
            TotalEarnings: 0, // Needs syncing from Store/Finance
            UpcomingAppointments: 0, // Needs syncing
            Rating: hakeem.Rating
        );

        return Result.Success(dashboard);
    }
}
