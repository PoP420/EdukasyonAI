using EdukasyonAI.Application.Contracts.Students.Dtos;

namespace EdukasyonAI.Maui.Models;

/// <summary>
/// Local SQLite model for offline student data caching.
/// Mirrors StudentProfileDto but with SQLite-friendly column types.
/// </summary>
public class LocalStudentProfile
{
    [SQLite.PrimaryKey, SQLite.AutoIncrement]
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int GradeLevel { get; set; }
    public float OverallMasteryScore { get; set; }
    public int TotalPointsEarned { get; set; }
    public int CurrentStreak { get; set; }
    public string PreferredLanguage { get; set; } = "Filipino";
    public int SyncStatus { get; set; }
    public DateTime? LastSyncedAt { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
