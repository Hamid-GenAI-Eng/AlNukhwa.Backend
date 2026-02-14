using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Misan.Modules.Identity.Domain.Entities;
using Misan.Modules.Identity.Infrastructure.Database;
using Misan.Shared.Kernel.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Identity.Application.Features.Auth.PasswordOps;

public record ResetPasswordCommand(string Email, string Token, string NewPassword) : IRequest<Result>;

public class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Token).NotEmpty(); // GUID or 6-digit code
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8);
    }
}

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IdentityDbContext _dbContext;

    public ResetPasswordCommandHandler(IdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
        if (user is null)
        {
             return Result.Failure(new Error("User.NotFound", "User not found."));
        }

        var otpToken = await _dbContext.OTPTokens
            .OrderByDescending(t => t.ExpiresOnUtc)
            .FirstOrDefaultAsync(t => t.UserId == user.Id && t.Token == request.Token && t.Type == "PasswordReset", cancellationToken);

        if (otpToken is null || otpToken.IsExpired || otpToken.IsUsed)
        {
             return Result.Failure(new Error("Token.Invalid", "Invalid or expired Token."));
        }

        otpToken.MarkAsUsed();
        
        var newHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.ChangePassword(newHash);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
