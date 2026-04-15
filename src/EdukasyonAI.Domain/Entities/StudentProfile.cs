using EdukasyonAI.Domain.Shared.Enums;

namespace EdukasyonAI.Domain.Entities;

/// <summary>
/// A student profile extending ApplicationUser with learning-specific data.
/// </summary>
public class StudentProfile : Entity
{
    public int UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public GradeLevel GradeLevel { get; set; }
    public int SchoolYear { get; set; } = DateTime.UtcNow.Year;

    // Adaptive learning state
    public float OverallMasteryScore { get; set; }
    public int TotalPointsEarned { get; set; }
    public int CurrentStreak { get; set; }
    public DateTime? LastActivityAt { get; set; }

    // Offline sync
    public SyncStatus SyncStatus { get; set; } = SyncStatus.PendingUpload;
    public DateTime? LastSyncedAt { get; set; }

    public ICollection<StudentProgress> ProgressRecords { get; set; } = new List<StudentProgress>();
    public ICollection<PracticeSession> PracticeSessions { get; set; } = new List<PracticeSession>();
}
