using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Misan.Modules.Identity.Application.Services;

namespace Misan.Modules.Identity.Infrastructure.Services;

public class ResendSettings
{
    public const string SectionName = "ResendSettings";
    public string ApiKey { get; init; } = null!;
    public string FromEmail { get; init; } = null!; // e.g., "onboarding@resend.dev" or your domain
}

public class ResendEmailService : IEmailService
{
    private readonly HttpClient _httpClient;
    private readonly ResendSettings _settings;
    private readonly ILogger<ResendEmailService> _logger;

    public ResendEmailService(IHttpClientFactory httpClientFactory, IOptions<ResendSettings> settings, ILogger<ResendEmailService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("Resend");
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        _logger.LogInformation("Attempting to send email via Resend to: {ExposedEmail}", to);

        var request = new
        {
            from = _settings.FromEmail,
            to = new[] { to },
            subject = subject,
            html = body
        };

        var response = await _httpClient.PostAsJsonAsync("https://api.resend.com/emails", request);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Resend API Failed. Status: {Status}. Response: {Content}", response.StatusCode, content);
            throw new Exception($"Failed to send email via Resend. Status: {response.StatusCode}. Response: {content}");
        }

        _logger.LogInformation("Resend API Success. Status: {Status}. Response: {Content}", response.StatusCode, content);
    }
}
