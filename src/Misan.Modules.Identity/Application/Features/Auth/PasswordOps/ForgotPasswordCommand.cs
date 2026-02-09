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

    public ForgotPasswordCommandHandler(IdentityDbContext dbContext, IEmailService emailService)
    {
        _dbContext = dbContext;
        _emailService = emailService;
    }

    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
        if (user is null)
        {
            return Result.Success();
        }

        var otpCode = new Random().Next(100000, 999999).ToString();
        var otpToken = OTPToken.Create(user.Id, otpCode, "PasswordReset", TimeSpan.FromMinutes(15));

        _dbContext.OTPTokens.Add(otpToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _emailService.SendEmailAsync(user.Email, "Reset Password", $"Your password reset code is: {otpCode}");

        return Result.Success();
    }
}
