using MediatR;
using Misan.Modules.Practitioner.Domain.Entities;
using Misan.Modules.Practitioner.Infrastructure.Database;
using Misan.Shared.Kernel.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Practitioner.Application.Features.Practitioner.Queries;

public record GetHakeemDetailQuery(Guid HakeemId) : IRequest<Result<Hakeem>>;

public class GetHakeemDetailQueryHandler : IRequestHandler<GetHakeemDetailQuery, Result<Hakeem>>
{
    private readonly PractitionerDbContext _context;

    public GetHakeemDetailQueryHandler(PractitionerDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Hakeem>> Handle(GetHakeemDetailQuery request, CancellationToken cancellationToken)
    {
        var hakeem = await _context.Hakeems
            .Include(h => h.Specializations)
            .Include(h => h.Languages)
            .Include(h => h.Qualifications)
            .Include(h => h.Documents.Where(d => d.Type != Domain.Enums.HakeemDocumentType.CNIC)) // Hide sensitive docs? Or show all if authorized? Assumed public profile hides sensitive.
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.Id == request.HakeemId, cancellationToken);

        if (hakeem == null)
            return Result.Failure<Hakeem>(new Error("Hakeem.NotFound", "Hakeem not found."));

        return Result.Success(hakeem);
    }
}
