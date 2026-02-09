using FluentValidation;
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

namespace Misan.Modules.Practitioner.Application.Features.Practitioner.Commands;

public record ScheduleDto(DayOfWeek DayOfWeek, TimeSpan MorningStart, TimeSpan MorningEnd, TimeSpan AfternoonStart, TimeSpan AfternoonEnd, bool IsClosed);

public record UpdateScheduleCommand(Guid HakeemId, Guid ClinicId, List<ScheduleDto> Schedules) : IRequest<Result>;

public class UpdateScheduleCommandHandler : IRequestHandler<UpdateScheduleCommand, Result>
{
    private readonly PractitionerDbContext _dbContext;

    public UpdateScheduleCommandHandler(PractitionerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(UpdateScheduleCommand request, CancellationToken cancellationToken)
    {
        var clinic = await _dbContext.Clinics
            .Include(c => c.Schedules)
            .FirstOrDefaultAsync(c => c.Id == request.ClinicId && c.HakeemId == request.HakeemId, cancellationToken);
        
        if (clinic is null) return Result.Failure(new Error("Clinic.NotFound", "Clinic not found for this Hakeem"));

        // Replace existing schedules
        // Since EF Core "owned" type support or collection replacement can be tricky, we remove old and add new.
        // Or if we have IDs in DTO, we update. Here we replace for simplicity (full schedule update).
        
        _dbContext.ClinicSchedules.RemoveRange(clinic.Schedules); // Remove existing
        
        foreach (var s in request.Schedules)
        {
            clinic.AddSchedule(s.DayOfWeek, s.MorningStart, s.MorningEnd, s.AfternoonStart, s.AfternoonEnd, s.IsClosed);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
