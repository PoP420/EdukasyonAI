using EdukasyonAI.Domain.Shared.Enums;

namespace EdukasyonAI.Domain.Entities;

/// <summary>
/// DepEd K-12 aligned course (e.g., Mathematics Grade 5).
/// </summary>
public class Course : Entity
{
    public string Title { get; set; } = string.Empty;
    public string TitleFilipino { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Subject Subject { get; set; }
    public GradeLevel GradeLevel { get; set; }
    public bool IsPremium { get; set; }
    public bool IsActive { get; set; } = true;

    // Multi-tenancy: null = shared across all schools
    public int? SchoolId { get; set; }
    public School? School { get; set; }

    public int CreatedByTeacherId { get; set; }

    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
