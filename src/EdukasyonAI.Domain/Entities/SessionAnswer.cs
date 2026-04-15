namespace EdukasyonAI.Domain.Entities;

/// <summary>
/// Records a student's individual answer within a PracticeSession.
/// </summary>
public class SessionAnswer : Entity
{
    public int PracticeSessionId { get; set; }
    public PracticeSession PracticeSession { get; set; } = null!;

    public int QuestionId { get; set; }
    public PracticeQuestion Question { get; set; } = null!;

    public string GivenAnswer { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int TimeSpentSeconds { get; set; }
    public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;
}
