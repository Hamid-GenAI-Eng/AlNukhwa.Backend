using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Misan.Modules.Clinical.Infrastructure.Database;

namespace Misan.Modules.Clinical;

public static class ClinicalModuleExtensions
{
    public static IServiceCollection AddClinicalModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<ClinicalDbContext>(options =>
            options.UseNpgsql(connectionString));
            
        // Register MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ClinicalModuleExtensions).Assembly));

        // Domain Services
        services.AddScoped<Misan.Modules.Clinical.Domain.Services.MizajCalculator>();
        services.AddScoped<Misan.Modules.Clinical.Infrastructure.Services.PrescriptionPdfService>();

        return services;
    }
}
