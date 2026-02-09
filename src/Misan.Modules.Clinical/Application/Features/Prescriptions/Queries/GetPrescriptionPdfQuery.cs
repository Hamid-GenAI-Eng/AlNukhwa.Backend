using MediatR;
using Microsoft.EntityFrameworkCore;
using Misan.Modules.Clinical.Infrastructure.Database;
using Misan.Modules.Clinical.Infrastructure.Services;
using Misan.Shared.Kernel.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Clinical.Application.Features.Prescriptions.Queries;

public record GetPrescriptionPdfQuery(Guid ConsultationId) : IRequest<Result<byte[]>>;

public class GetPrescriptionPdfQueryHandler : IRequestHandler<GetPrescriptionPdfQuery, Result<byte[]>>
{
    private readonly ClinicalDbContext _dbContext;
    private readonly PrescriptionPdfService _pdfService;

    public GetPrescriptionPdfQueryHandler(ClinicalDbContext dbContext, PrescriptionPdfService pdfService)
    {
        _dbContext = dbContext;
        _pdfService = pdfService;
    }

    public async Task<Result<byte[]>> Handle(GetPrescriptionPdfQuery request, CancellationToken cancellationToken)
    {
        var prescription = await _dbContext.Prescriptions
            .Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.ConsultationId == request.ConsultationId, cancellationToken);

        if (prescription == null)
            return Result.Failure<byte[]>(new Error("Prescription.NotFound", "No prescription found for this consultation."));

        // Ideal: Fetch Hakeem/Patient names via other modules (Query Bus or HTTP)
        // For MVP: Placeholders
        var hakeemName = "Dr. Hakeem Placeholder";
        var patientName = "Patient Placeholder";

        var pdfBytes = _pdfService.GeneratePrescription(prescription, hakeemName, patientName);

        return Result.Success(pdfBytes);
    }
}
