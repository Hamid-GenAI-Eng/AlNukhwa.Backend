using MediatR;
using Misan.Modules.Clinical.Domain.Entities;
using Misan.Modules.Clinical.Infrastructure.Database;
using Misan.Shared.Kernel.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Clinical.Application.Features.Prescriptions.Commands;

public record PrescriptionItemDto(string Remedy, string Dosage, string Instructions);

public record CreatePrescriptionCommand(Guid ConsultationId, List<PrescriptionItemDto> Items) : IRequest<Result<Guid>>;

public class CreatePrescriptionCommandHandler : IRequestHandler<CreatePrescriptionCommand, Result<Guid>>
{
    private readonly ClinicalDbContext _dbContext;

    public CreatePrescriptionCommandHandler(ClinicalDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Guid>> Handle(CreatePrescriptionCommand request, CancellationToken cancellationToken)
    {
        var prescription = Prescription.Create(request.ConsultationId);

        foreach (var item in request.Items)
        {
            prescription.AddItem(item.Remedy, item.Dosage, item.Instructions);
        }

        _dbContext.Prescriptions.Add(prescription);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(prescription.Id);
    }
}
