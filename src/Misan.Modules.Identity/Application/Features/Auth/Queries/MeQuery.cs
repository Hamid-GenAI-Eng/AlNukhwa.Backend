using MediatR;
using Microsoft.EntityFrameworkCore;
using Misan.Modules.Identity.Infrastructure.Database;
using Misan.Shared.Kernel.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Identity.Application.Features.Auth.Queries;

public record MeQuery(Guid UserId) : IRequest<Result<MeResponse>>;

public record MeResponse(Guid Id, string Email, string Phone, string Role, bool IsEmailVerified);

public class MeQueryHandler : IRequestHandler<MeQuery, Result<MeResponse>>
{
    private readonly IdentityDbContext _dbContext;

    public MeQueryHandler(IdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<MeResponse>> Handle(MeQuery request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.FindAsync(new object[] { request.UserId }, cancellationToken);
        if (user is null)
        {
             return Result.Failure<MeResponse>(new Error("User.NotFound", "User not found."));
        }

        return new MeResponse(user.Id, user.Email, user.Phone, user.Role.ToString(), user.IsEmailVerified);
    }
}
