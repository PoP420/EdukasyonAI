using EdukasyonAI.Domain.Shared.Enums;

namespace EdukasyonAI.Application.Contracts.Students.Dtos;

public class RegisterStudentDto
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public GradeLevel GradeLevel { get; set; }
    public int? SchoolId { get; set; }
    public Language PreferredLanguage { get; set; } = Language.Filipino;
}
