using EdukasyonAI.Domain.Shared.Enums;

namespace EdukasyonAI.Domain.Entities;

/// <summary>
/// Represents a school tenant.  Multi-tenancy provides school-level data isolation.
/// </summary>
public class School : Entity
{
    public string Name { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string Division { get; set; } = string.Empty;
    public string DepEdSchoolId { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    public ICollection<Course> Courses { get; set; } = new List<Course>();
}
