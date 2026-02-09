using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Misan.Modules.Identity.Application.Services;
using Misan.Modules.Identity.Domain.Entities;
using Misan.Modules.Identity.Infrastructure.Database;
using Misan.Shared.Kernel.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Identity.Application.Features.Auth.ResendOtp;

public record ResendOtpCommand(string Email) : IRequest<Result>;

public class ResendOtpValidator : AbstractValidator<ResendOtpCommand>
{
    public ResendOtpValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}

public class ResendOtpCommandHandler : IRequestHandler<ResendOtpCommand, Result>
{
    private readonly IdentityDbContext _dbContext;
    private readonly IEmailService _emailService;

    public ResendOtpCommandHandler(IdentityDbContext dbContext, IEmailService emailService)
    {
        _dbContext = dbContext;
        _emailService = emailService;
    }

    public async Task<Result> Handle(ResendOtpCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
        if (user is null)
        {
            // Don't reveal user existence
            return Result.Success();
        }

        var otpCode = new Random().Next(100000, 999999).ToString();
        var otpToken = OTPToken.Create(user.Id, otpCode, "Email", TimeSpan.FromMinutes(10));

        _dbContext.OTPTokens.Add(otpToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _emailService.SendEmailAsync(user.Email, "Your OTP Code", $"Your verification code is: {otpCode}");

        return Result.Success();
    }
}
