using Microsoft.EntityFrameworkCore;
using Misan.Modules.Identity.Infrastructure.Database;
using Misan.Modules.Profiles.Infrastructure.Database;
using Misan.Modules.Practitioner.Infrastructure.Database;
using Misan.Modules.Clinical.Infrastructure.Database;
using Misan.Modules.Store.Infrastructure.Database;
using Misan.Modules.Intelligence.Infrastructure.Database;

namespace Misan.Bootstrapper.Infrastructure.Database;

public static class MigrationHelper
{
    public static async Task ApplyMigrationsAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();

        try
        {
            logger.LogInformation("Applying migrations...");

            await MigrateDbContextAsync<IdentityDbContext>(services, logger);
            await MigrateDbContextAsync<ProfilesDbContext>(services, logger);
            await MigrateDbContextAsync<PractitionerDbContext>(services, logger);
            await MigrateDbContextAsync<ClinicalDbContext>(services, logger);
            await MigrateDbContextAsync<StoreDbContext>(services, logger);
            await MigrateDbContextAsync<IntelligenceDbContext>(services, logger);

            logger.LogInformation("Migrations applied successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while applying migrations.");
            throw;
        }
    }

    private static async Task MigrateDbContextAsync<T>(IServiceProvider services, ILogger logger) where T : DbContext
    {
        var context = services.GetRequiredService<T>();
        if ((await context.Database.GetPendingMigrationsAsync()).Any())
        {
            logger.LogInformation($"Applying pending migrations for {typeof(T).Name}...");
            await context.Database.MigrateAsync();
        }
    }
}
