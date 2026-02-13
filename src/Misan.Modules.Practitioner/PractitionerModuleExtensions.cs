using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Misan.Modules.Practitioner.Infrastructure.Database;
using Misan.Modules.Practitioner.Infrastructure.Services;
using Misan.Modules.Practitioner.Application.Services;

namespace Misan.Modules.Practitioner;

public static class PractitionerModuleExtensions
{
    public static IServiceCollection AddPractitionerModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<PractitionerDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Add MediatR (if needed locally, usually global registration covers it if assemblies scanned)
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(PractitionerModuleExtensions).Assembly));

        // Cloudinary
        services.Configure<CloudinarySettings>(configuration.GetSection(CloudinarySettings.SectionName));
        services.AddScoped<IImageService, CloudinaryService>();

        return services;
    }
}
