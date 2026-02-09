using MediatR;
using FluentValidation;
using Misan.Shared.Kernel.Abstractions;
using Misan.Modules.Identity.Domain.Entities;
using Misan.Modules.Identity.Domain.Enums;
using Misan.Modules.Identity.Infrastructure.Database;
using BCrypt.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Misan.Modules.Identity.Application.Features.Auth.Register;

public record RegisterPatientCommand(string Email, string Password, string Phone) : IRequest<Result<Guid>>;

public class RegisterPatientValidator : AbstractValidator<RegisterPatientCommand>
{
    public RegisterPatientValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.Phone).NotEmpty().Matches(@"^\+?[1-9]\d{1,14}$");
    }
}

public class RegisterPatientCommandHandler : IRequestHandler<RegisterPatientCommand, Result<Guid>>
{
    private readonly IdentityDbContext _dbContext;
    private readonly Application.Services.IEmailService _emailService;
    private readonly IPublisher _publisher;

    public RegisterPatientCommandHandler(IdentityDbContext dbContext, Application.Services.IEmailService emailService, IPublisher publisher)
    {
        _dbContext = dbContext;
        _emailService = emailService;
        _publisher = publisher;
    }

    public async Task<Result<Guid>> Handle(RegisterPatientCommand request, CancellationToken cancellationToken)
    {
        if (await _dbContext.Users.AnyAsync(u => u.Email == request.Email, cancellationToken))
        {
            return Result.Failure<Guid>(new Error("User.EmailNotUnique", "The email is already in use."));
        }

        if (await _dbContext.Users.AnyAsync(u => u.Phone == request.Phone, cancellationToken))
        {
            return Result.Failure<Guid>(new Error("User.PhoneNotUnique", "The phone number is already in use."));
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = User.Create(request.Email, request.Phone, passwordHash, UserRole.Patient);

        _dbContext.Users.Add(user);
        
        // Generate OTP
        var otp = new Random().Next(100000, 999999).ToString();
        var otpToken = OTPToken.Create(user.Id, otp, "Email", TimeSpan.FromMinutes(10));
        _dbContext.OTPTokens.Add(otpToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        // Send Email
        // Note: In a real production app, this should be done via a background job/queue to ensure reliability and speed.
        try 
        {
            await _emailService.SendEmailAsync(user.Email, "Misan Verification Code", $"Your verification code is: <b>{otp}</b>");
        }
        catch (Exception ex)
        {
            // Log failure but don't fail the registration? Or fail? 
            // For now, logging error via Serilog is implicit if we let it bubble or catch & log.
            // Let's log and not break registration for now, but user needs to hit Resend OTP.
            Serilog.Log.Error(ex, "Failed to send OTP email to {Email}", user.Email);
        }

        // Publish UserRegisteredEvent for other modules (Profiles)
        await _publisher.Publish(new Misan.Shared.Kernel.IntegrationEvents.UserRegisteredEvent(user.Id, user.Email, user.Role.ToString()), cancellationToken);

        return user.Id;
    }
}
