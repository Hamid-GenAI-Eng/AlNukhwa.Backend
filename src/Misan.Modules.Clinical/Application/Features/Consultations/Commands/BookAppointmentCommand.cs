using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Misan.Modules.Clinical.Domain.Entities;
using Misan.Modules.Clinical.Domain.Enums;
using Misan.Modules.Clinical.Infrastructure.Database;
using Misan.Shared.Kernel.Abstractions;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Clinical.Application.Features.Consultations.Commands;

public record BookAppointmentCommand(
    Guid PatientId,
    Guid HakeemId,
    DateTime ScheduledAt,
    ConsultationType Type) : IRequest<Result<Guid>>;

public class BookAppointmentValidator : AbstractValidator<BookAppointmentCommand>
{
    public BookAppointmentValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.HakeemId).NotEmpty();
        RuleFor(x => x.ScheduledAt).GreaterThan(DateTime.UtcNow).WithMessage("Time must be in future");
    }
}

public class BookAppointmentCommandHandler : IRequestHandler<BookAppointmentCommand, Result<Guid>>
{
    private readonly ClinicalDbContext _dbContext;

    public BookAppointmentCommandHandler(ClinicalDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Guid>> Handle(BookAppointmentCommand request, CancellationToken cancellationToken)
    {
        // 1. Race Condition Prevention: Use Serializable Transaction
        // This ensures that if two requests try to book the same slot simultaneously,
        // one will fail with a concurrency exception or be serialized safely.
        
        using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

        try
        {
            // 2. Check for existing appointment for this Hakeem at this Time
            // Precision: Assuming 1-hour slots or similar. Exact match for simplicity now.
            var existing = await _dbContext.Consultations
                .AnyAsync(c => c.HakeemId == request.HakeemId && c.ScheduledAt == request.ScheduledAt && c.Status != ConsultationStatus.Cancelled, cancellationToken);

            if (existing)
            {
                return Result.Failure<Guid>(new Error("Appointment.Conflict", "Slot is already booked."));
            }

            // 3. Validation: Check Hakeem Shift/Schedule (Skipped for now, assuming FE passes valid slots)
            // Ideally call Practitioner Module to validate availability.

            // 4. Create Consultation
            var consultation = Consultation.Schedule(
                request.PatientId,
                request.HakeemId,
                request.ScheduledAt,
                request.Type,
                1500 // Placeholder fee, should fetch from Hakeem/Clinic
            );

            _dbContext.Consultations.Add(consultation);
            await _dbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return Result.Success(consultation.Id);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            // Log ex
            return Result.Failure<Guid>(new Error("Appointment.Failed", ex.Message));
        }
    }
}
