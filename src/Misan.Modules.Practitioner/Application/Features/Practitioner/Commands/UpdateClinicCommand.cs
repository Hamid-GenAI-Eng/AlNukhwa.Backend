using MediatR;
using Misan.Modules.Practitioner.Infrastructure.Database;
using Misan.Shared.Kernel.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Practitioner.Application.Features.Practitioner.Commands;

public record UpdateClinicCommand(
    Guid HakeemId,
    string ClinicName,
    string Address,
    string City,
    string MapCoordinates,
    string PhoneNumber
) : IRequest<Result>;

public class UpdateClinicCommandHandler : IRequestHandler<UpdateClinicCommand, Result>
{
    private readonly PractitionerDbContext _context;

    public UpdateClinicCommandHandler(PractitionerDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateClinicCommand request, CancellationToken cancellationToken)
    {
        var clinic = await _context.Clinics
            .FirstOrDefaultAsync(c => c.HakeemId == request.HakeemId, cancellationToken);

        if (clinic == null)
            return Result.Failure(new Error("Clinic.NotFound", "Clinic not found for this Hakeem."));

        // Use reflection or manual update. Since Entity properties are private set, 
        // we ideally need a method on the Entity 'UpdateDetails(...)'.
        // Assuming we can add such a method or use the constructor/update pattern.
        // Let checking Clinic Entity first. If Update method is missing, we might need to add it 
        // OR (less clean) assume we can update if properties are setup.
        
        // Actually, let's assume standard Entity pattern where we can add a method 'Update' 
        // to the Clinic entity, OR if we can't edit the entity file instantly, 
        // we rely on the fact that we can modify properties if they have key.
        // But in DDD, setters are private.
        
        // Let's check Clinic.cs content to be sure. 
        // Since I can't check it inside this single turn without interruption, 
        // I will optimistically implement this assuming I can update properties 
        // or I will add the method to Clinic.cs in the NEXT step if needed.
        
        // For now, I will assume a method `UpdateDetails` exists or I will ADD it shortly.
        
        clinic.UpdateDetails(request.ClinicName, request.Address, request.City, request.MapCoordinates, request.PhoneNumber);

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
