using Microsoft.EntityFrameworkCore;
using Misan.Modules.Practitioner.Domain.Entities;

namespace Misan.Modules.Practitioner.Infrastructure.Database;

public class PractitionerDbContext : DbContext
{
    public PractitionerDbContext(DbContextOptions<PractitionerDbContext> options) : base(options)
    {
    }

    public DbSet<Hakeem> Hakeems { get; set; } = null!;
    public DbSet<HakeemSpecialization> HakeemSpecializations { get; set; } = null!;
    public DbSet<HakeemLanguage> HakeemLanguages { get; set; } = null!;
    public DbSet<HakeemQualification> HakeemQualifications { get; set; } = null!;
    public DbSet<HakeemDocument> HakeemDocuments { get; set; } = null!;
    
    public DbSet<Clinic> Clinics { get; set; } = null!;
    public DbSet<ClinicService> ClinicServices { get; set; } = null!;
    public DbSet<ClinicFee> ClinicFees { get; set; } = null!;
    public DbSet<ClinicSchedule> ClinicSchedules { get; set; } = null!;
    
    public DbSet<ScheduleBreak> ScheduleBreaks { get; set; } = null!;
    public DbSet<ScheduleConfig> ScheduleConfigs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PractitionerDbContext).Assembly);
        modelBuilder.HasDefaultSchema("practitioner");
    }
}
