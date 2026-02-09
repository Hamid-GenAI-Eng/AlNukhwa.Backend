using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Misan.Modules.Profiles.Application.Services;
using Misan.Modules.Profiles.Infrastructure.Database;
using Misan.Modules.Profiles.Infrastructure.Services;
using Misan.Shared.Kernel.Abstractions;

namespace Misan.Modules.Profiles;

public static class ProfilesModuleExtensions
{
    public static IServiceCollection AddProfilesModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ProfilesDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Database")));
            
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ProfilesModuleExtensions).Assembly));
        services.AddValidatorsFromAssembly(typeof(ProfilesModuleExtensions).Assembly);

        services.Configure<CloudinarySettings>(configuration.GetSection(CloudinarySettings.SectionName));
        services.AddScoped<IImageService, CloudinaryService>();

        services.AddScoped<IPatientHealthService, PatientHealthService>();
        
        return services;
    }
}
