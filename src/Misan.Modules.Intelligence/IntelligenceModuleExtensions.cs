using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Misan.Modules.Intelligence.Infrastructure.Database;
using Misan.Modules.Intelligence.Infrastructure.Services;
using System.Reflection;

namespace Misan.Modules.Intelligence;

public static class IntelligenceModuleExtensions
{
    public static IServiceCollection AddIntelligenceModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<IntelligenceDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        services.AddSignalR();
        
        // Register FastAPI Client
        services.AddHttpClient<IFastApiService, FastApiService>(client =>
        {
            client.BaseAddress = new System.Uri(configuration["FastApi:BaseUrl"] ?? "http://localhost:8000");
        });

        return services;
    }
}
