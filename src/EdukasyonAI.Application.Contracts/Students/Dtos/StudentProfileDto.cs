using EdukasyonAI.Domain.Shared.Enums;

namespace EdukasyonAI.Application.Contracts.Students.Dtos;

public class StudentProfileDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public GradeLevel GradeLevel { get; set; }
    public int SchoolYear { get; set; }
    public float OverallMasteryScore { get; set; }
    public int TotalPointsEarned { get; set; }
    public int CurrentStreak { get; set; }
    public DateTime? LastActivityAt { get; set; }
    public Language PreferredLanguage { get; set; }
    public SyncStatus SyncStatus { get; set; }
    public DateTime? LastSyncedAt { get; set; }
}
