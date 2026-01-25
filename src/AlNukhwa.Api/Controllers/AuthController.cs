using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AlNukhwa.Application.DTOs;
using AlNukhwa.Domain.Entities;
using AlNukhwa.Infrastructure.Services;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AlNukhwa.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _config;
    private readonly IEmailService _emailService;

    public AuthController(UserManager<ApplicationUser> userManager, IConfiguration config, IEmailService emailService)
    {
        _userManager = userManager;
        _config = config;
        _emailService = emailService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var existing = await _userManager.FindByEmailAsync(request.Email);
        if (existing != null) return BadRequest("Email already exists.");

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded) return BadRequest(result.Errors);

        await SendOtpInternal(user);
        return Ok("Registration successful. Please verify your OTP sent to email.");
    }

    [HttpPost("send-otp")]
    public async Task<IActionResult> SendOtp(SendOtpRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) return NotFound("User not found.");
        await SendOtpInternal(user);
        return Ok("OTP sent successfully.");
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp(VerifyOtpRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || user.OtpCode != request.Otp || user.OtpExpiry < DateTime.UtcNow)
            return BadRequest("Invalid or expired OTP.");

        user.EmailVerified = true;
        user.OtpCode = null;
        await _userManager.UpdateAsync(user);

        await _emailService.SendEmailAsync(user.Email!, "Welcome to Al-Nukhwa", "<h1>Congratulations!</h1><p>Your account is verified.</p>");
        
        return Ok(GenerateAuthResponse(user));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            return Unauthorized("Invalid credentials.");

        if (!user.EmailVerified) return BadRequest("Email not verified.");

        return Ok(GenerateAuthResponse(user));
    }

    [HttpPost("google-login")]
    public async Task<IActionResult> GoogleLogin(GoogleLoginRequest request)
    {
        var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken);
        var user = await _userManager.FindByEmailAsync(payload.Email);

        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = payload.Email,
                Email = payload.Email,
                FirstName = payload.GivenName,
                LastName = payload.FamilyName,
                EmailVerified = true,
                GoogleId = payload.Subject
            };
            await _userManager.CreateAsync(user);
        }

        return Ok(GenerateAuthResponse(user));
    }

    private async Task SendOtpInternal(ApplicationUser user)
    {
        var otp = new Random().Next(100000, 999999).ToString();
        user.OtpCode = otp;
        user.OtpExpiry = DateTime.UtcNow.AddMinutes(5);
        await _userManager.UpdateAsync(user);
        await _emailService.SendEmailAsync(user.Email!, "Your Al-Nukhwa OTP", $"Your OTP is: {otp}. Valid for 5 minutes.");
    }

    private AuthResponse GenerateAuthResponse(ApplicationUser user)
    {
        var claims = new[] {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Audience"], claims, expires: DateTime.UtcNow.AddDays(7), signingCredentials: creds);

        return new AuthResponse(new JwtSecurityTokenHandler().WriteToken(token), user.Email!, user.FirstName, user.LastName);
    }
}