using Microsoft.EntityFrameworkCore;
using Misan.Modules.Profiles.Infrastructure.Database;
using Misan.Shared.Kernel.Abstractions;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Misan.Modules.Profiles.Infrastructure.Services;

public class PatientHealthService : IPatientHealthService
{
    private readonly ProfilesDbContext _dbContext;

    public PatientHealthService(ProfilesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string> GetHealthContextAsync(Guid userId, CancellationToken cancellationToken)
    {
        var profile = await _dbContext.Profiles
            .Include(p => p.HealthProfile).ThenInclude(hp => hp.Conditions)
            .Include(p => p.HealthProfile).ThenInclude(hp => hp.Allergies)
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

        if (profile == null) return "No health profile found.";

        var sb = new StringBuilder();
        sb.AppendLine($"Name: {profile.FullName}");
        sb.AppendLine($"Age: {CalculateAge(profile.Dob)}");
        sb.AppendLine($"Gender: {profile.Gender}");
        
        if (profile.HealthProfile != null)
        {
            sb.AppendLine($"Blood Group: {profile.HealthProfile.BloodGroup}");
            sb.AppendLine($"Body Type: {profile.HealthProfile.BodyType}");
            
            if (profile.HealthProfile.Conditions.Any())
            {
                sb.AppendLine("Conditions: " + string.Join(", ", profile.HealthProfile.Conditions.Select(c => c.Name))); // Assuming Condition has Name
            }

            if (profile.HealthProfile.Allergies.Any())
            {
                sb.AppendLine("Allergies: " + string.Join(", ", profile.HealthProfile.Allergies.Select(a => a.Name))); 
            }
        }

        return sb.ToString();
    }

    private string CalculateAge(DateTime? dob)
    {
        if (!dob.HasValue) return "Unknown";
        var age = DateTime.Today.Year - dob.Value.Year;
        if (dob.Value.Date > DateTime.Today.AddYears(-age)) age--;
        return age.ToString();
    }
}
