using EdukasyonAI.Domain.Shared.Enums;

namespace EdukasyonAI.Application.Contracts.Students.Dtos;

public class StudentProgressDto
{
    public int Id { get; set; }
    public int LessonId { get; set; }
    public string LessonTitle { get; set; } = string.Empty;
    public ProgressStatus Status { get; set; }
    public float MasteryScore { get; set; }
    public int AttemptsCount { get; set; }
    public int CorrectAnswers { get; set; }
    public int TotalAnswers { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? LastAttemptAt { get; set; }
}
