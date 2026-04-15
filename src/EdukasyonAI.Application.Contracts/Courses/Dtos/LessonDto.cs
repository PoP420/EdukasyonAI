using EdukasyonAI.Domain.Shared.Enums;

namespace EdukasyonAI.Application.Contracts.Courses.Dtos;

public class LessonDto
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string TitleFilipino { get; set; } = string.Empty;
    public string ContentMarkdown { get; set; } = string.Empty;
    public string ContentMarkdownFilipino { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public int EstimatedMinutes { get; set; }
    public bool IsAiGenerated { get; set; }
}
