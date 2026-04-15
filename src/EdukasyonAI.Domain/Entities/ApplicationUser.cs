using EdukasyonAI.Domain.Shared.Enums;

namespace EdukasyonAI.Domain.Entities;

/// <summary>
/// Application user — shared base for Student, Teacher and Parent identities.
/// Complies with Philippines Data Privacy Act (RA 10173) by storing only
/// necessary personal data and flagging under-13 users for COPPA handling.
/// </summary>
public class ApplicationUser : Entity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public UserRole Role { get; set; }
    public Language PreferredLanguage { get; set; } = Language.Filipino;
    public bool IsActive { get; set; } = true;
    public bool RequiresParentalConsent { get; set; }
    public DateTime? ConsentGrantedAt { get; set; }

    // Multi-tenancy: school the user belongs to
    public int? SchoolId { get; set; }
    public School? School { get; set; }

    // Refresh token for JWT auth
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiresAt { get; set; }

    public bool IsUnder13 => DateOfBirth.HasValue &&
        (DateTime.UtcNow - DateOfBirth.Value).TotalDays / 365.25 < 13;
}
