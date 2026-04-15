using EdukasyonAI.Domain.Shared.Enums;

namespace EdukasyonAI.Domain.Entities;

/// <summary>
/// Tracks a student's mastery of a specific lesson.
/// Used for adaptive learning to decide next recommended content.
/// </summary>
public class StudentProgress : Entity
{
    public int StudentProfileId { get; set; }
    public StudentProfile StudentProfile { get; set; } = null!;

    public int LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;

    public ProgressStatus Status { get; set; } = ProgressStatus.NotStarted;
    public float MasteryScore { get; set; }       // 0.0 – 1.0
    public int AttemptsCount { get; set; }
    public int CorrectAnswers { get; set; }
    public int TotalAnswers { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? LastAttemptAt { get; set; }

    public SyncStatus SyncStatus { get; set; } = SyncStatus.PendingUpload;
    public DateTime? LastSyncedAt { get; set; }
}
