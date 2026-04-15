using EdukasyonAI.Domain.Shared.Enums;

namespace EdukasyonAI.Domain.Entities;

/// <summary>
/// A timed practice session — one or more questions answered by a student.
/// </summary>
public class PracticeSession : Entity
{
    public int StudentProfileId { get; set; }
    public StudentProfile StudentProfile { get; set; } = null!;

    public int LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;

    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? FinishedAt { get; set; }
    public int Score { get; set; }
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }

    // Adaptive difficulty used for this session
    public DifficultyLevel Difficulty { get; set; }

    public SyncStatus SyncStatus { get; set; } = SyncStatus.PendingUpload;
    public DateTime? LastSyncedAt { get; set; }

    public ICollection<SessionAnswer> Answers { get; set; } = new List<SessionAnswer>();
}
