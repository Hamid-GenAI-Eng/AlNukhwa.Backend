using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Misan.Modules.Identity.Infrastructure.Database;
using FluentValidation;
using Misan.Modules.Identity.Infrastructure.Authentication;
using Misan.Modules.Identity.Infrastructure.Services;
using Misan.Modules.Identity.Application.Services;

namespace Misan.Modules.Identity;

public static class IdentityModuleExtensions
{
    public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<IdentityDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Database")));

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IdentityModuleExtensions).Assembly));
        services.AddValidatorsFromAssembly(typeof(IdentityModuleExtensions).Assembly);

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName));

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IEmailService, SmtpEmailService>();

        return services;
    }
}
