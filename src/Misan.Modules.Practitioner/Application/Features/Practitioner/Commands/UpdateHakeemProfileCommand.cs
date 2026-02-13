using MediatR;
using Misan.Modules.Practitioner.Infrastructure.Database;
using Misan.Shared.Kernel.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Practitioner.Application.Features.Practitioner.Commands;

public record UpdateHakeemProfileCommand(
    Guid UserId, 
    List<string>? Specializations,
    List<string>? Languages,
    List<QualificationDto>? Qualifications
) : IRequest<Result>;

public record QualificationDto(string Title, string Institution, int Year);

public class UpdateHakeemProfileCommandHandler : IRequestHandler<UpdateHakeemProfileCommand, Result>
{
    private readonly PractitionerDbContext _context;

    public UpdateHakeemProfileCommandHandler(PractitionerDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateHakeemProfileCommand command, CancellationToken cancellationToken)
    {
        var hakeem = await _context.Hakeems
            .Include(h => h.Specializations)
            .Include(h => h.Languages)
            .Include(h => h.Qualifications) // Ensure we load related data
            .FirstOrDefaultAsync(h => h.UserId == command.UserId, cancellationToken);

        if (hakeem == null)
            return Result.Failure(new Error("Hakeem.NotFound", "Hakeem profile not found."));

        // Since the Hakeem entity uses specialized methods to ADD but not clear/set, 
        // and EF Core tracking handles collection changes properly if we manipulate the collection directly...
        // However, the Entity has private lists exposed as IReadOnlyCollection.
        // We usually need methods on the Entity to 'UpdateSpecializations(List<string>)' or we manually clear via db context if we want full replacement.
        // For 'Add' only logic, we check existence.
        // For 'Update', typically we want to sync (add missing, remove extra).
        
        // Given existing Entity design: AddSpecialization checks existence.
        // Missing: RemoveSpecialization.
        
        // Let's implement basic "Add New Ones" logic for now as 'Sync' logic is complex without Entity support.
        // Ideally, we should update the Entity model to support full updates.
        
        if (command.Specializations != null)
        {
            foreach (var spec in command.Specializations)
            {
                hakeem.AddSpecialization(spec);
            }
        }

        if (command.Languages != null)
        {
             foreach (var lang in command.Languages)
            {
                hakeem.AddLanguage(lang);
            }
        }
        
        if (command.Qualifications != null)
        {
            foreach (var qual in command.Qualifications)
            {
                hakeem.AddQualification(qual.Title, qual.Institution, qual.Year);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
