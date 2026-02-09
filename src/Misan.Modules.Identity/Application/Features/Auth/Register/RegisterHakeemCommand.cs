using MediatR;
using FluentValidation;
using Misan.Shared.Kernel.Abstractions;
using Misan.Modules.Identity.Domain.Entities;
using Misan.Modules.Identity.Domain.Enums;
using Misan.Modules.Identity.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Identity.Application.Features.Auth.Register;

public record RegisterHakeemCommand(
    string Email, 
    string Password, 
    string Phone, 
    string LicenseNumber, // Extra verification field
    int YearsOfExperience
) : IRequest<Result<Guid>>;

public class RegisterHakeemValidator : AbstractValidator<RegisterHakeemCommand>
{
    public RegisterHakeemValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.Phone).NotEmpty();
        RuleFor(x => x.LicenseNumber).NotEmpty();
        RuleFor(x => x.YearsOfExperience).GreaterThan(0);
    }
}

public class RegisterHakeemCommandHandler : IRequestHandler<RegisterHakeemCommand, Result<Guid>>
{
    private readonly IdentityDbContext _dbContext;
    private readonly Application.Services.IEmailService _emailService;
    private readonly IPublisher _publisher;

    public RegisterHakeemCommandHandler(IdentityDbContext dbContext, Application.Services.IEmailService emailService, IPublisher publisher)
    {
        _dbContext = dbContext;
        _emailService = emailService;
        _publisher = publisher;
    }

    public async Task<Result<Guid>> Handle(RegisterHakeemCommand request, CancellationToken cancellationToken)
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

        var user = User.Create(request.Email, request.Phone, passwordHash, UserRole.Hakeem);

        _dbContext.Users.Add(user);
        
        // Generate OTP
        var otp = new Random().Next(100000, 999999).ToString();
        var otpToken = OTPToken.Create(user.Id, otp, "Email", TimeSpan.FromMinutes(10));
        _dbContext.OTPTokens.Add(otpToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        // Send Email
        try 
        {
            await _emailService.SendEmailAsync(user.Email, "Misan Verification Code", $"Your verification code is: <b>{otp}</b>");
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "Failed to send OTP email to {Email}", user.Email);
        }

        // Publish UserRegisteredEvent
        await _publisher.Publish(new Misan.Shared.Kernel.IntegrationEvents.UserRegisteredEvent(user.Id, user.Email, user.Role.ToString()), cancellationToken);

        return user.Id;
    }
}
