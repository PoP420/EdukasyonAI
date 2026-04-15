namespace EdukasyonAI.Domain.Entities;

/// <summary>
/// Base entity with an integer primary key and audit timestamps.
/// </summary>
public abstract class Entity
{
    public int Id { get; protected set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
