using EdukasyonAI.Domain.Shared.Enums;

namespace EdukasyonAI.Domain.Entities;

/// <summary>
/// A single learning unit within a Course.
/// Content is stored bilingual (English + Filipino) for Taglish support.
/// </summary>
public class Lesson : Entity
{
    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string TitleFilipino { get; set; } = string.Empty;
    public string ContentMarkdown { get; set; } = string.Empty;
    public string ContentMarkdownFilipino { get; set; } = string.Empty;

    public int OrderIndex { get; set; }
    public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Beginner;
    public int EstimatedMinutes { get; set; } = 15;

    // AI-generated flag for content moderation
    public bool IsAiGenerated { get; set; }
    public bool IsApprovedByTeacher { get; set; }

    public ICollection<PracticeQuestion> Questions { get; set; } = new List<PracticeQuestion>();
    public ICollection<StudentProgress> ProgressRecords { get; set; } = new List<StudentProgress>();
}
