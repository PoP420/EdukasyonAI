using EdukasyonAI.Domain.Shared.Enums;

namespace EdukasyonAI.Domain.Entities;

public enum QuestionType
{
    MultipleChoice,
    TrueOrFalse,
    FillInTheBlank,
    ShortAnswer,
    Matching
}

/// <summary>
/// AI-generated or teacher-authored practice question.
/// Stored with age-appropriate content guardrails.
/// </summary>
public class PracticeQuestion : Entity
{
    public int LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;

    public string QuestionText { get; set; } = string.Empty;
    public string QuestionTextFilipino { get; set; } = string.Empty;
    public QuestionType Type { get; set; }
    public DifficultyLevel Difficulty { get; set; }

    // JSON-serialized list of choices for MultipleChoice/Matching
    public string ChoicesJson { get; set; } = "[]";
    public string CorrectAnswer { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
    public string ExplanationFilipino { get; set; } = string.Empty;

    public bool IsAiGenerated { get; set; }
    public bool IsApproved { get; set; }

    // Safety: flagged for review if AI content is potentially inappropriate
    public bool IsFlaggedForReview { get; set; }
}
