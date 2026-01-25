namespace AlNukhwa.Application.DTOs;

public record RegisterRequest(string FirstName, string LastName, string Email, string Password);
public record LoginRequest(string Email, string Password);
public record VerifyOtpRequest(string Email, string Otp);
public record SendOtpRequest(string Email);
public record GoogleLoginRequest(string IdToken);
public record AuthResponse(string Token, string Email, string FirstName, string LastName);