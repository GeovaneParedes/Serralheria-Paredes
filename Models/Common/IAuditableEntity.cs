namespace Serralheria_Paredes.Models.Common;

/// <summary>
/// Contrato para entidades auditáveis que rastreiam carimbo de data/hora de criação e última modificação.
/// Elimina a necessidade de inspeção por reflexão no ChangeTracker do DbContext.
/// </summary>
public interface IAuditableEntity
{
    DateTime CreatedAt { get; set; }
    DateTime? UpdatedAt { get; set; }
}
