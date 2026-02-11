using System.Net.Http.Json;
using System.Text.Json;
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

    public ResendEmailService(IHttpClientFactory httpClientFactory, IOptions<ResendSettings> settings)
    {
        _httpClient = httpClientFactory.CreateClient("Resend");
        _settings = settings.Value;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var request = new
        {
            from = _settings.FromEmail,
            to = new[] { to },
            subject = subject,
            html = body
        };

        var response = await _httpClient.PostAsJsonAsync("https://api.resend.com/emails", request);
        
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to send email via Resend. Status: {response.StatusCode}. Response: {content}");
        }
    }
}
