using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Misan.Modules.Practitioner.Domain.Entities;
using Misan.Modules.Practitioner.Domain.Enums;
using Misan.Modules.Practitioner.Infrastructure.Database;
using Misan.Shared.Kernel.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Practitioner.Application.Features.Practitioner.Commands;

public record VerifyDocumentCommand(Guid HakeemId, Guid DocumentId, bool Approve, string? RejectionReason) : IRequest<Result>;

public class VerifyDocumentCommandHandler : IRequestHandler<VerifyDocumentCommand, Result>
{
    private readonly PractitionerDbContext _dbContext;

    public VerifyDocumentCommandHandler(PractitionerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(VerifyDocumentCommand request, CancellationToken cancellationToken)
    {
        var hakeem = await _dbContext.Hakeems
            .Include(h => h.Documents)
            .FirstOrDefaultAsync(h => h.Id == request.HakeemId, cancellationToken);

        if (hakeem is null) return Result.Failure(new Error("Hakeem.NotFound", "Hakeem not found"));

        var document = await _dbContext.HakeemDocuments.FindAsync(new object[] { request.DocumentId }, cancellationToken);
        if (document is null) return Result.Failure(new Error("Document.NotFound", "Document not found"));

        // Logic: Approving a document marks it as verified.
        // If ALL required docs (Certificate, License, CNIC) are verified, auto-verify Hakeem?
        // Or manual final step?
        // Let's implement auto-check for now.

        if (request.Approve)
        {
            document.Verify();
            
            // Check if Hakeem can be fully verified
            // Requirement: "Verification Queue: Admin approval system for Hakeem documents."
            // Implicitly, verification depends on docs.
            
            // Simplistic rule: If CNIC and License are verified, verify Hakeem.
            var hasVerifiedCnic = hakeem.Documents.Any(d => d.Type == HakeemDocumentType.CNIC && (d.Id == document.Id || d.Verified));
            var hasVerifiedLicense = hakeem.Documents.Any(d => d.Type == HakeemDocumentType.License && (d.Id == document.Id || d.Verified));

            if (hasVerifiedCnic && hasVerifiedLicense)
            {
                hakeem.Verify();
            }
        }
        else
        {
            // Rejection logic - maybe delete document or mark status?
            // Current entity has simple boolean "Verified". 
            // Might need "Rejected" state on document, but Entity only has bool.
            // For now, if rejected, we remove it or leave unverified? 
            // Better: Delete so they upload again.
            _dbContext.HakeemDocuments.Remove(document);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
