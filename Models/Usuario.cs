using Microsoft.AspNetCore.Identity;

namespace Serralheria_Paredes.Models;

/// <summary>
/// Entidade de Usuário do sistema de gestão da Serralheria.
/// Herda de IdentityUser<int> para suporte nativo a autenticação, hashing robusto e auditoria.
/// </summary>
public class Usuario : IdentityUser<int>
{
    public string NomeCompleto { get; set; } = string.Empty;
    public string? Cargo { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
