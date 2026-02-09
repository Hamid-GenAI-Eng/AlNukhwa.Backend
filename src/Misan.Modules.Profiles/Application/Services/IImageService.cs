using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Misan.Shared.Kernel.Abstractions;

namespace Misan.Modules.Profiles.Application.Services;

public interface IImageService
{
    Task<Result<string>> UploadImageAsync(IFormFile file, string folder);
    Task<Result> DeleteImageAsync(string publicId);
}
