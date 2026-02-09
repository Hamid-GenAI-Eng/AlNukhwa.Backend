using Microsoft.EntityFrameworkCore;
using Misan.Modules.Clinical.Domain.Entities;

namespace Misan.Modules.Clinical.Infrastructure.Database;

public class ClinicalDbContext : DbContext
{
    public ClinicalDbContext(DbContextOptions<ClinicalDbContext> options) : base(options) { }

    public DbSet<Consultation> Consultations { get; set; }
    public DbSet<ConsultationNote> ConsultationNotes { get; set; }
    public DbSet<ConsultationAttachment> ConsultationAttachments { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<PrescriptionItem> PrescriptionItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("clinical");

        modelBuilder.Entity<Consultation>(b => {
            b.HasKey(c => c.Id);
            b.HasMany(c => c.Notes).WithOne().HasForeignKey(n => n.ConsultationId).OnDelete(DeleteBehavior.Cascade);
            b.HasMany(c => c.Attachments).WithOne().HasForeignKey(a => a.ConsultationId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Prescription>(b => {
            b.HasKey(p => p.Id);
            b.HasMany(p => p.Items).WithOne().HasForeignKey(i => i.PrescriptionId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
