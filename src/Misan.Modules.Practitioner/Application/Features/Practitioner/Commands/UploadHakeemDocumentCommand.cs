using MediatR;
using Microsoft.AspNetCore.Http;
using Misan.Modules.Practitioner.Application.Services;
using Misan.Modules.Practitioner.Domain.Enums;
using Misan.Modules.Practitioner.Infrastructure.Database;
using Misan.Shared.Kernel.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Practitioner.Application.Features.Practitioner.Commands;

public record UploadHakeemDocumentCommand(Guid UserId, IFormFile File, HakeemDocumentType DocumentType) : IRequest<Result<string>>;

public class UploadHakeemDocumentCommandHandler : IRequestHandler<UploadHakeemDocumentCommand, Result<string>>
{
    private readonly PractitionerDbContext _context;
    private readonly IImageService _imageService;

    public UploadHakeemDocumentCommandHandler(PractitionerDbContext context, IImageService imageService)
    {
        _context = context;
        _imageService = imageService;
    }

    public async Task<Result<string>> Handle(UploadHakeemDocumentCommand request, CancellationToken cancellationToken)
    {
        var hakeem = await _context.Hakeems
            .FirstOrDefaultAsync(h => h.UserId == request.UserId, cancellationToken);

        if (hakeem == null)
            return Result.Failure<string>(new Error("Hakeem.NotFound", "Hakeem profile not found."));

        // Use appropriate folder structure
        var folder = $"hakeems/{hakeem.Id}/documents";
        
        var uploadResult = await _imageService.UploadImageAsync(request.File, folder);
        if (uploadResult.IsFailure) return Result.Failure<string>(uploadResult.Error);

        var url = uploadResult.Value;

        hakeem.AddDocument(request.DocumentType, url);
        
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(url);
    }
}
