using Microsoft.EntityFrameworkCore;
using Misan.Modules.Intelligence.Domain.Entities;

namespace Misan.Modules.Intelligence.Infrastructure.Database;

public class IntelligenceDbContext : DbContext
{
    public IntelligenceDbContext(DbContextOptions<IntelligenceDbContext> options) : base(options) { }

    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<ConversationParticipant> ConversationParticipants { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<AIChatSession> AIChatSessions { get; set; }
    public DbSet<AIChatMessage> AIChatMessages { get; set; }
    public DbSet<AIChatFeedback> AIChatFeedbacks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("intelligence");

        modelBuilder.Entity<Conversation>(b => {
            b.HasKey(c => c.Id);
            b.HasMany(c => c.Participants).WithOne(p => p.Conversation).HasForeignKey(p => p.ConversationId).OnDelete(DeleteBehavior.Cascade);
            b.HasMany(c => c.Messages).WithOne().HasForeignKey(m => m.ConversationId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ConversationParticipant>(b => {
             b.HasKey(p => p.Id);
             b.HasIndex(p => new { p.ConversationId, p.UserId }).IsUnique();
        });

        modelBuilder.Entity<Message>(b => {
            b.HasKey(m => m.Id);
            b.HasIndex(m => m.ConversationId);
            b.HasIndex(m => m.SentAt);
        });

        modelBuilder.Entity<AIChatSession>(b => {
            b.HasKey(s => s.Id);
            b.HasMany(s => s.Messages).WithOne().HasForeignKey(m => m.SessionId).OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(s => s.UserId);
        });

        modelBuilder.Entity<AIChatMessage>(b => {
            b.HasKey(m => m.Id);
            b.HasMany(m => m.Feedback).WithOne().HasForeignKey(f => f.MessageId).OnDelete(DeleteBehavior.Cascade);
            b.Navigation(m => m.Feedback).UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<AIChatFeedback>(b => {
            b.HasKey(f => f.Id);
        });
    }
}
