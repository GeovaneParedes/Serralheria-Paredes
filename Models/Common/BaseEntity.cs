namespace Serralheria_Paredes.Models.Common;

/// <summary>
/// Entidade base abstrata para todas as entidades persistidas no PostgreSQL.
/// Garante auditoria básica (CreatedAt, UpdatedAt) e identificador único.
/// </summary>
public abstract class BaseEntity<TKey>
{
    public TKey Id { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
}

public abstract class BaseEntity : BaseEntity<int>
{
}
