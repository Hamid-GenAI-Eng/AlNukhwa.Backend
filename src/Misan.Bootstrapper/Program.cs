using Misan.Bootstrapper.Middleware;
using Misan.Modules.Identity;
using Misan.Modules.Practitioner;
using Misan.Modules.Clinical;
using Misan.Modules.Profiles;
using Misan.Modules.Store;
using Misan.Modules.Intelligence;
using Misan.Shared.Kernel.Behaviors;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Misan.Modules.Identity.Infrastructure.Authentication;
using Misan.Bootstrapper.Infrastructure.Database; // MigrationHelper

var builder = WebApplication.CreateBuilder(args);

// --- 1. Environment Variable Configuration Mapping ---
// Helper to read Env Var or Config
string? GetConfig(string envKey, string configKey) => 
    Environment.GetEnvironmentVariable(envKey) ?? builder.Configuration[configKey];

// Database URL Parsing (Render format: postgres://user:pass@host:port/db)
// Database URL Parsing
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL") ?? builder.Configuration.GetConnectionString("Database");
string connectionString = "";

if (!string.IsNullOrEmpty(databaseUrl) && (databaseUrl.StartsWith("postgres://") || databaseUrl.StartsWith("postgresql://")))
{
    try 
    {
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':');
        var port = uri.Port == -1 ? 5432 : uri.Port;
        connectionString = $"Host={uri.Host};Port={port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error parsing Database URI: {ex.Message}");
        // Fallback or let it fail
        connectionString = databaseUrl; // Attempt to use as-is if parsing fails
    }
}
else
{
    connectionString = databaseUrl ?? "";
}

// Inject Connection String into Configuration for Modules to pick up
builder.Configuration["ConnectionStrings:Database"] = connectionString;

// Map Cloudinary
builder.Configuration["CloudinarySettings:CloudName"] = GetConfig("CLOUDINARY_cloudname", "CloudinarySettings:CloudName");
builder.Configuration["CloudinarySettings:ApiKey"] = GetConfig("CLOUDINARY_apikey", "CloudinarySettings:ApiKey");
builder.Configuration["CloudinarySettings:ApiSecret"] = GetConfig("CLOUDINARY_apisecret", "CloudinarySettings:ApiSecret");

// Map Resend Email
builder.Configuration["ResendSettings:ApiKey"] = GetConfig("RESEND_API_KEY", "ResendSettings:ApiKey");
builder.Configuration["ResendSettings:FromEmail"] = GetConfig("RESEND_FROM_EMAIL", "ResendSettings:FromEmail");

// Map PayFast (Future Proofing)
builder.Configuration["PayFastSettings:MerchantId"] = GetConfig("PAYFAST_MERCHANT_ID", "PayFastSettings:MerchantId");
builder.Configuration["PayFastSettings:SecurePass"] = GetConfig("PAYFAST_SECURE_PASS", "PayFastSettings:SecurePass");


// --- Serilog ---
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// --- Services ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks(); // Health Check

// Global Exception Handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// --- Modules ---
// Pass the updated configuration
builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddProfilesModule(builder.Configuration);
builder.Services.AddPractitionerModule(builder.Configuration);
builder.Services.AddClinicalModule(builder.Configuration);
builder.Services.AddStoreModule(builder.Configuration);
builder.Services.AddIntelligenceModule(builder.Configuration);

// --- CORS ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => {
            var frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL") ?? "http://localhost:3000";
            policy.WithOrigins(frontendUrl, "http://localhost:3000", "https://al-nukhwa-frontend.vercel.app") // Add generic Vercel fallback
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });

    // SignalR specific policy if needed, or merge with above
    options.AddPolicy("SignalRPolicy",
        policy => {
             var frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL") ?? "http://localhost:3000";
             policy.WithOrigins(frontendUrl, "http://localhost:3000")
             .AllowAnyMethod()
             .AllowAnyHeader()
             .AllowCredentials();
        });
});

// --- Auth ---
var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
        };
    });

var app = builder.Build();

// --- 2. Automatic Migrations ---
// Run migrations within a scope
using (var scope = app.Services.CreateScope())
{
    // Check if we should skip migrations (optional flag)
    if (Environment.GetEnvironmentVariable("SKIP_MIGRATIONS") != "true")
    {
        await MigrationHelper.ApplyMigrationsAsync(scope.ServiceProvider);
    }
}

// --- Middleware ---
app.UseExceptionHandler();

// Use Serilog Request Logging
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// CORS - Must be before Auth
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// Health Check Endpoint
app.MapHealthChecks("/health");

app.MapControllers();
app.MapHub<Misan.Modules.Intelligence.Infrastructure.SignalR.ChatHub>("/ws/messages").RequireCors("SignalRPolicy");

app.Run();
