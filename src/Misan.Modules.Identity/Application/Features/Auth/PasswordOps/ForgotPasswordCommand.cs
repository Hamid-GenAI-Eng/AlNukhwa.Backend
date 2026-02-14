using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Misan.Modules.Identity.Application.Services;
using Misan.Modules.Identity.Domain.Entities;
using Misan.Modules.Identity.Infrastructure.Database;
using Misan.Shared.Kernel.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Identity.Application.Features.Auth.PasswordOps;

public record ForgotPasswordCommand(string Email) : IRequest<Result>;

public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly IdentityDbContext _dbContext;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;

    public ForgotPasswordCommandHandler(IdentityDbContext dbContext, IEmailService emailService, IConfiguration configuration, ILogger<ForgotPasswordCommandHandler> logger)
    {
        _dbContext = dbContext;
        _emailService = emailService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing Forgot Password request for: {Email}", request.Email);
        
        var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("User not found for email: {Email}. Returning success to prevent enumeration.", request.Email);
            return Result.Success();
        }

        _logger.LogInformation("User found. Generating secure token for: {UserId}", user.Id);

        // Generate Secure Token (GUID) instead of 6-digit OTP
        var token = Guid.NewGuid().ToString();
        var otpToken = OTPToken.Create(user.Id, token, "PasswordReset", TimeSpan.FromMinutes(15));

        _dbContext.OTPTokens.Add(otpToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Construct Link
        var frontendUrl = _configuration["FRONTEND_URL"] ?? "http://localhost:3000";
        // Ensure not trailing slash if needed, but modern browsers handle it. 
        // frontendUrl/reset-password?email=...&token=...
        var resetLink = $"{frontendUrl}/reset-password?email={Uri.EscapeDataString(user.Email)}&token={token}";

        var emailBody = $@"
            <h2>Reset Password Request</h2>
            <p>You have requested to reset your password. Click the link below to verify your identity and set a new password:</p>
            <p><a href='{resetLink}'>Reset Password</a></p>
            <p>If you did not request this, please ignore this email.</p>
            <p>This link expires in 15 minutes.</p>
        ";

        await _emailService.SendEmailAsync(user.Email, "Reset Your Password - Al-Nukhwa", emailBody);

        return Result.Success();
    }
}
