using EdukasyonAI.Domain.Shared.Enums;

namespace EdukasyonAI.Application.Contracts.Courses.Dtos;

public class CourseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string TitleFilipino { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Subject Subject { get; set; }
    public GradeLevel GradeLevel { get; set; }
    public bool IsPremium { get; set; }
    public int LessonCount { get; set; }
}
