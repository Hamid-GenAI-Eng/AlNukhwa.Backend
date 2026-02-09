using System;
using System.Security.Claims;
using Misan.Modules.Identity.Domain.Entities;

namespace Misan.Modules.Identity.Application.Services;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}
