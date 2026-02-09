using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; // Add this
using Misan.Modules.Store.Infrastructure.Database;

namespace Misan.Modules.Store;

public static class StoreExtensions
{
    public static IServiceCollection AddStoreModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<StoreDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(StoreExtensions).Assembly));

        services.AddScoped<Misan.Modules.Store.Application.Services.IPaymentService, Misan.Modules.Store.Infrastructure.Services.MockPayfastService>();

        return services;
    }
}
