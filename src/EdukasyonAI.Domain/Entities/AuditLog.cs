namespace EdukasyonAI.Domain.Entities;

/// <summary>
/// Audit log entry for student and teacher activity (RA 10173 compliance).
/// </summary>
public class AuditLog : Entity
{
    public int? UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? IpAddress { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}
