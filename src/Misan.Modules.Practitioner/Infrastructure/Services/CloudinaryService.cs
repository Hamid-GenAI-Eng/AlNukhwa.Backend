using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Misan.Modules.Practitioner.Application.Services;
using Misan.Shared.Kernel.Abstractions;
using System.Threading.Tasks;

namespace Misan.Modules.Practitioner.Infrastructure.Services;

public class CloudinarySettings
{
    public const string SectionName = "CloudinarySettings";
    public string CloudName { get; init; } = null!;
    public string ApiKey { get; init; } = null!;
    public string ApiSecret { get; init; } = null!;
}

public class CloudinaryService : IImageService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IOptions<CloudinarySettings> settings)
    {
        var account = new Account(
            settings.Value.CloudName,
            settings.Value.ApiKey,
            settings.Value.ApiSecret);

        _cloudinary = new Cloudinary(account);
    }

    public async Task<Result<string>> UploadImageAsync(IFormFile file, string folder)
    {
        if (file.Length == 0) return Result.Failure<string>(new Misan.Shared.Kernel.Abstractions.Error("File.Empty", "File is empty"));

        using var stream = file.OpenReadStream();
        // Determine resource type based on extension if needed, but 'auto' works well usually.
        // For strict video handling, we might need VideoUploadParams, but 'ImageUploadParams' handles most.
        // Actually, for Generic Documents (PDF), we need 'RawUploadParams' or strictly specify 'Resource_type'.
        
        // Simple logic: if video -> VideoUploadParams, else ImageUploadParams
        // But for this task, 'documents/images/video'. 
        
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = folder,
            Overwrite = true
        };

        // Note: Cloudinary DotNet SDK can handle video with ImageUploadParams for some cases, 
        // but robust implementation would distinguish. Keeping it simple as per previous Profiles implementation.
        // Improvement: Use Raw for PDF.

        var extension = System.IO.Path.GetExtension(file.FileName).ToLower();
        if (extension == ".pdf" || extension == ".doc" || extension == ".docx")
        {
             var rawParams = new RawUploadParams
             {
                 File = new FileDescription(file.FileName, stream),
                 Folder = folder,
                 Overwrite = true
             };
             var rawResult = await _cloudinary.UploadAsync(rawParams);
             if (rawResult.Error != null) return Result.Failure<string>(new Misan.Shared.Kernel.Abstractions.Error("Upload.Failed", rawResult.Error.Message));
             return rawResult.SecureUrl.ToString();
        }

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error != null)
        {
            return Result.Failure<string>(new Misan.Shared.Kernel.Abstractions.Error("Upload.Failed", uploadResult.Error.Message));
        }

        return uploadResult.SecureUrl.ToString();
    }

    public async Task<Result> DeleteImageAsync(string publicId)
    {
        var deletionParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deletionParams);

        if (result.Error != null)
        {
             return Result.Failure(new Misan.Shared.Kernel.Abstractions.Error("Delete.Failed", result.Error.Message));
        }

        return Result.Success();
    }
}
