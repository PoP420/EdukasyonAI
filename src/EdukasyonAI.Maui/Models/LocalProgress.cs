namespace EdukasyonAI.Maui.Models;

/// <summary>
/// Local SQLite model for offline lesson progress caching.
/// </summary>
public class LocalProgress
{
    [SQLite.PrimaryKey, SQLite.AutoIncrement]
    public int Id { get; set; }
    public int StudentProfileId { get; set; }
    public int LessonId { get; set; }
    public string LessonTitle { get; set; } = string.Empty;
    public int Status { get; set; }       // maps to ProgressStatus enum
    public float MasteryScore { get; set; }
    public int AttemptsCount { get; set; }
    public int CorrectAnswers { get; set; }
    public int TotalAnswers { get; set; }
    public DateTime? LastAttemptAt { get; set; }
    public int SyncStatus { get; set; }   // maps to SyncStatus enum
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
