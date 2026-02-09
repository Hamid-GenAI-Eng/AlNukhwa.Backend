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

// Add Condition
// Add Condition
public record AddConditionCommand(Guid UserId, string Name, Domain.Enums.Severity Severity, DateTime DiagnosedDate) : IRequest<Result>;

public class AddConditionCommandHandler : IRequestHandler<AddConditionCommand, Result>
{
    private readonly ProfilesDbContext _dbContext;

    public AddConditionCommandHandler(ProfilesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(AddConditionCommand request, CancellationToken cancellationToken)
    {
        var profile = await _dbContext.Profiles
            .Include(p => p.HealthProfile)
            .FirstOrDefaultAsync(p => p.UserId == request.UserId, cancellationToken);

        if (profile?.HealthProfile is null) return Result.Failure(new Error("HealthProfile.NotFound", "Health Profile not found."));

        // Fix: Ensure DiagnosedDate is UTC for Npgsql
        var diagnosedDateUtc = DateTime.SpecifyKind(request.DiagnosedDate, DateTimeKind.Utc);
        
        var condition = new Condition(Guid.NewGuid(), profile.HealthProfile.Id, request.Name, request.Severity, diagnosedDateUtc);
        _dbContext.Conditions.Add(condition);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}

// Remove Condition
public record RemoveConditionCommand(Guid UserId, Guid ConditionId) : IRequest<Result>;

public class RemoveConditionCommandHandler : IRequestHandler<RemoveConditionCommand, Result>
{
    private readonly ProfilesDbContext _dbContext;

    public RemoveConditionCommandHandler(ProfilesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(RemoveConditionCommand request, CancellationToken cancellationToken)
    {
        var condition = await _dbContext.Conditions.FindAsync(new object[] { request.ConditionId }, cancellationToken);
        if (condition is null) return Result.Failure(new Error("Condition.NotFound", "Condition not found"));

        _dbContext.Conditions.Remove(condition);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

// Add Allergy
// Add Allergy
public record AddAllergyCommand(Guid UserId, string Name, Domain.Enums.Severity Severity, string Description) : IRequest<Result>;

public class AddAllergyCommandHandler : IRequestHandler<AddAllergyCommand, Result>
{
    private readonly ProfilesDbContext _dbContext;

    public AddAllergyCommandHandler(ProfilesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(AddAllergyCommand request, CancellationToken cancellationToken)
    {
        var profile = await _dbContext.Profiles
            .Include(p => p.HealthProfile)
            .FirstOrDefaultAsync(p => p.UserId == request.UserId, cancellationToken);

        if (profile?.HealthProfile is null) return Result.Failure(new Error("HealthProfile.NotFound", "Health Profile not found."));

        // if (!Enum.TryParse<Domain.Enums.Severity>(request.Severity, true, out var severity)) return Result.Failure(new Error("Validation.InvalidSeverity", "Invalid Severity"));

        var allergy = new Allergy(Guid.NewGuid(), profile.HealthProfile.Id, request.Name, request.Severity, request.Description);
        _dbContext.Allergies.Add(allergy);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

// Update Emergency Contact
public record UpdateEmergencyContactCommand(Guid UserId, string Name, string Relationship, string Phone) : IRequest<Result>;

public class UpdateEmergencyContactCommandHandler : IRequestHandler<UpdateEmergencyContactCommand, Result>
{
    private readonly ProfilesDbContext _dbContext;

    public UpdateEmergencyContactCommandHandler(ProfilesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(UpdateEmergencyContactCommand request, CancellationToken cancellationToken)
    {
        var profile = await _dbContext.Profiles
            .Include(p => p.HealthProfile) // Not needed really, but loading profile
            .FirstOrDefaultAsync(p => p.UserId == request.UserId, cancellationToken);

        if (profile is null) return Result.Failure(new Error("Profile.NotFound", "Profile not found"));

        var contact = await _dbContext.EmergencyContacts.FirstOrDefaultAsync(c => c.ProfileId == profile.Id, cancellationToken);
        
        if (contact is null)
        {
            contact = new EmergencyContact(Guid.NewGuid(), profile.Id, request.Name, request.Relationship, request.Phone);
            _dbContext.EmergencyContacts.Add(contact);
        }
        else
        {
            // EmergencyContact entity is immutable-ish in my definition (private setters maybe?), let's see.
            // Entity code: properties were get-only with constructor init. 
            // I should update Entity to allow updates or Replace.
            // For now, I'll delete and re-add or check Entity definition.
            // Entity definition: "public string Name { get; private set; }" - so I can't update directly without a method.
            // I should add Update method to EmergencyContact or Replace logic.
            // I'll assume I can just Remove and Add new since it's "UpdateEmergencyContact" (Singular) implying one contact?
            // User requirement "PUT /api/profile/emergency-contact".
            
            _dbContext.EmergencyContacts.Remove(contact);
            var newContact = new EmergencyContact(Guid.NewGuid(), profile.Id, request.Name, request.Relationship, request.Phone);
            _dbContext.EmergencyContacts.Add(newContact);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
