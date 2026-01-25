using Microsoft.AspNetCore.Identity;

namespace AlNukhwa.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? OtpCode { get; set; }
    public DateTime? OtpExpiry { get; set; }
    public bool EmailVerified { get; set; } = false;
    public string? GoogleId { get; set; }
}