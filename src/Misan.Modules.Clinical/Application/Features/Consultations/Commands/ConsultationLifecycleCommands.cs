using MediatR;
using Microsoft.EntityFrameworkCore;
using Misan.Modules.Clinical.Domain.Entities;
using Misan.Modules.Clinical.Infrastructure.Database;
using Misan.Shared.Kernel.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Clinical.Application.Features.Consultations.Commands;

// Reschedule
public record RescheduleConsultationCommand(Guid ConsultationId, DateTime NewDate) : IRequest<Result>;

public class RescheduleConsultationCommandHandler : IRequestHandler<RescheduleConsultationCommand, Result>
{
    private readonly ClinicalDbContext _dbContext;
    public RescheduleConsultationCommandHandler(ClinicalDbContext dbContext) => _dbContext = dbContext;

    public async Task<Result> Handle(RescheduleConsultationCommand request, CancellationToken cancellationToken)
    {
        var consultation = await _dbContext.Consultations.FindAsync(new object[] { request.ConsultationId }, cancellationToken);
        if (consultation == null) return Result.Failure(new Error("Consultation.NotFound", "Not found"));

        consultation.Reschedule(request.NewDate);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

// Cancel
public record CancelConsultationCommand(Guid ConsultationId) : IRequest<Result>;

public class CancelConsultationCommandHandler : IRequestHandler<CancelConsultationCommand, Result>
{
    private readonly ClinicalDbContext _dbContext;
    public CancelConsultationCommandHandler(ClinicalDbContext dbContext) => _dbContext = dbContext;

    public async Task<Result> Handle(CancelConsultationCommand request, CancellationToken cancellationToken)
    {
        var consultation = await _dbContext.Consultations.FindAsync(new object[] { request.ConsultationId }, cancellationToken);
        if (consultation == null) return Result.Failure(new Error("Consultation.NotFound", "Not found"));

        consultation.Cancel();
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

// Complete
public record CompleteConsultationCommand(Guid ConsultationId) : IRequest<Result>;

public class CompleteConsultationCommandHandler : IRequestHandler<CompleteConsultationCommand, Result>
{
    private readonly ClinicalDbContext _dbContext;
    public CompleteConsultationCommandHandler(ClinicalDbContext dbContext) => _dbContext = dbContext;

    public async Task<Result> Handle(CompleteConsultationCommand request, CancellationToken cancellationToken)
    {
        var consultation = await _dbContext.Consultations.FindAsync(new object[] { request.ConsultationId }, cancellationToken);
        if (consultation == null) return Result.Failure(new Error("Consultation.NotFound", "Not found"));

        consultation.Complete();
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

// Add Note
public record AddConsultationNoteCommand(Guid ConsultationId, string Text, bool IsPrivate) : IRequest<Result>;

public class AddConsultationNoteCommandHandler : IRequestHandler<AddConsultationNoteCommand, Result>
{
    private readonly ClinicalDbContext _dbContext;
    public AddConsultationNoteCommandHandler(ClinicalDbContext dbContext) => _dbContext = dbContext;

    public async Task<Result> Handle(AddConsultationNoteCommand request, CancellationToken cancellationToken)
    {
        var consultation = await _dbContext.Consultations
            .Include(c => c.Notes)
            .FirstOrDefaultAsync(c => c.Id == request.ConsultationId, cancellationToken);
            
        if (consultation == null) return Result.Failure(new Error("Consultation.NotFound", "Not found"));

        consultation.AddNote(request.Text, request.IsPrivate);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
