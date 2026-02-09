using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Misan.Modules.Identity.Domain.Entities;
using Misan.Modules.Identity.Infrastructure.Database;
using Misan.Shared.Kernel.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Identity.Application.Features.Auth.VerifyOtp;

public record VerifyOtpCommand(string Email, string Otp) : IRequest<Result>;

public class VerifyOtpValidator : AbstractValidator<VerifyOtpCommand>
{
    public VerifyOtpValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Otp).NotEmpty().Length(6);
    }
}

public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, Result>
{
    private readonly IdentityDbContext _dbContext;

    public VerifyOtpCommandHandler(IdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
        if (user is null)
        {
            return Result.Failure(new Error("User.NotFound", "User not found."));
        }

        var otpToken = await _dbContext.OTPTokens
            .OrderByDescending(t => t.ExpiresOnUtc)
            .FirstOrDefaultAsync(t => t.UserId == user.Id && t.Token == request.Otp && t.Type == "Email", cancellationToken);

        if (otpToken is null)
        {
             return Result.Failure(new Error("Otp.Invalid", "Invalid OTP."));
        }

        if (otpToken.IsExpired)
        {
             return Result.Failure(new Error("Otp.Expired", "OTP has expired."));
        }

        if (otpToken.IsUsed)
        {
             return Result.Failure(new Error("Otp.Used", "OTP has already been used."));
        }

        otpToken.MarkAsUsed();
        user.VerifyEmail();

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
