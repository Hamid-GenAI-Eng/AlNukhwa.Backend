using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Misan.Modules.Practitioner.Domain.Entities;
using Misan.Modules.Practitioner.Domain.Enums;

namespace Misan.Modules.Practitioner.Infrastructure.Database.Configurations;

public class HakeemConfiguration : IEntityTypeConfiguration<Hakeem>
{
    public void Configure(EntityTypeBuilder<Hakeem> builder)
    {
        builder.HasKey(h => h.Id);
        builder.HasIndex(h => h.UserId).IsUnique(); // One Hakeem profile per User
        builder.Property(h => h.CnicNumber).HasMaxLength(20);
        builder.Property(h => h.LicenseNumber).HasMaxLength(50);
        builder.Property(h => h.VerificationStatus).HasConversion<string>();

        // Owned Collections (Simplified as related entities for now due to clean separation)
        builder.HasMany(h => h.Specializations).WithOne().HasForeignKey(s => s.HakeemId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(h => h.Languages).WithOne().HasForeignKey(l => l.HakeemId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(h => h.Qualifications).WithOne().HasForeignKey(q => q.HakeemId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(h => h.Documents).WithOne().HasForeignKey(d => d.HakeemId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class ClinicConfiguration : IEntityTypeConfiguration<Clinic>
{
    public void Configure(EntityTypeBuilder<Clinic> builder)
    {
        builder.HasKey(c => c.Id);
        // Hakeem can have multiple clinics? "Clinic Management structure: clinics table has hakeem_id". 
        // User request: "Multi-clinic support per Hakeem".
        builder.HasIndex(c => c.HakeemId); 

        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        
        builder.HasMany(c => c.Services).WithOne().HasForeignKey(s => s.ClinicId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(c => c.Fees).WithOne().HasForeignKey(f => f.ClinicId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(c => c.Schedules).WithOne().HasForeignKey(s => s.ClinicId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class ScheduleConfigConfiguration : IEntityTypeConfiguration<ScheduleConfig>
{
    public void Configure(EntityTypeBuilder<ScheduleConfig> builder)
    {
        builder.HasKey(sc => sc.Id);
        builder.HasIndex(sc => sc.HakeemId).IsUnique(); // One config per Hakeem
    }
}
