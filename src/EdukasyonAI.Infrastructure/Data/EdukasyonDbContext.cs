using EdukasyonAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EdukasyonAI.Infrastructure.Data;

/// <summary>
/// EF Core DbContext — supports both SQLite (offline/mobile) and PostgreSQL (cloud sync).
/// </summary>
public class EdukasyonDbContext : DbContext
{
    public EdukasyonDbContext(DbContextOptions<EdukasyonDbContext> options) : base(options) { }

    public DbSet<School> Schools => Set<School>();
    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
    public DbSet<StudentProfile> StudentProfiles => Set<StudentProfile>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<PracticeQuestion> PracticeQuestions => Set<PracticeQuestion>();
    public DbSet<StudentProgress> StudentProgresses => Set<StudentProgress>();
    public DbSet<PracticeSession> PracticeSessions => Set<PracticeSession>();
    public DbSet<SessionAnswer> SessionAnswers => Set<SessionAnswer>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // School
        modelBuilder.Entity<School>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(256).IsRequired();
            e.Property(x => x.DepEdSchoolId).HasMaxLength(64);
            e.HasIndex(x => x.DepEdSchoolId).IsUnique();
        });

        // ApplicationUser
        modelBuilder.Entity<ApplicationUser>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Email).HasMaxLength(256).IsRequired();
            e.Property(x => x.Username).HasMaxLength(64).IsRequired();
            e.Property(x => x.FullName).HasMaxLength(128).IsRequired();
            e.HasIndex(x => x.Email).IsUnique();
            e.HasIndex(x => x.Username).IsUnique();
            e.HasOne(x => x.School)
             .WithMany(s => s.Users)
             .HasForeignKey(x => x.SchoolId)
             .OnDelete(DeleteBehavior.SetNull);
        });

        // StudentProfile
        modelBuilder.Entity<StudentProfile>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.User)
             .WithOne()
             .HasForeignKey<StudentProfile>(x => x.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // Course
        modelBuilder.Entity<Course>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Title).HasMaxLength(256).IsRequired();
            e.Property(x => x.TitleFilipino).HasMaxLength(256);
            e.HasOne(x => x.School)
             .WithMany(s => s.Courses)
             .HasForeignKey(x => x.SchoolId)
             .OnDelete(DeleteBehavior.SetNull);
        });

        // Lesson
        modelBuilder.Entity<Lesson>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Title).HasMaxLength(256).IsRequired();
            e.HasOne(x => x.Course)
             .WithMany(c => c.Lessons)
             .HasForeignKey(x => x.CourseId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // PracticeQuestion
        modelBuilder.Entity<PracticeQuestion>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.QuestionText).HasMaxLength(2000).IsRequired();
            e.HasOne(x => x.Lesson)
             .WithMany(l => l.Questions)
             .HasForeignKey(x => x.LessonId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // StudentProgress
        modelBuilder.Entity<StudentProgress>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.StudentProfileId, x.LessonId }).IsUnique();
            e.HasOne(x => x.StudentProfile)
             .WithMany(s => s.ProgressRecords)
             .HasForeignKey(x => x.StudentProfileId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Lesson)
             .WithMany(l => l.ProgressRecords)
             .HasForeignKey(x => x.LessonId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // PracticeSession
        modelBuilder.Entity<PracticeSession>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.StudentProfile)
             .WithMany(s => s.PracticeSessions)
             .HasForeignKey(x => x.StudentProfileId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // SessionAnswer
        modelBuilder.Entity<SessionAnswer>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.PracticeSession)
             .WithMany(s => s.Answers)
             .HasForeignKey(x => x.PracticeSessionId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Question)
             .WithMany()
             .HasForeignKey(x => x.QuestionId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // AuditLog
        modelBuilder.Entity<AuditLog>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Action).HasMaxLength(128).IsRequired();
            e.Property(x => x.EntityType).HasMaxLength(128);
        });
    }
}
