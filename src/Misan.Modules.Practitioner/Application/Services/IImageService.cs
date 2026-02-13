using Microsoft.AspNetCore.Http;
using Misan.Shared.Kernel.Abstractions;
using System.Threading.Tasks;

namespace Misan.Modules.Practitioner.Application.Services;

public interface IImageService
{
    Task<Result<string>> UploadImageAsync(IFormFile file, string folder);
    Task<Result> DeleteImageAsync(string publicId);
}
