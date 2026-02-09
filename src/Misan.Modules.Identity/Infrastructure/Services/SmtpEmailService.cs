using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using Misan.Modules.Identity.Application.Services;
using System.Threading.Tasks;

namespace Misan.Modules.Identity.Infrastructure.Services;

public class EmailSettings
{
    public const string SectionName = "EmailSettings";
    public string SmtpServer { get; init; } = null!;
    public int SmtpPort { get; init; }
    public string SenderName { get; init; } = null!;
    public string SenderEmail { get; init; } = null!;
    public string Password { get; init; } = null!;
}

public class SmtpEmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public SmtpEmailService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;

        message.Body = new TextPart("html")
        {
            Text = body
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_settings.SenderEmail, _settings.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
