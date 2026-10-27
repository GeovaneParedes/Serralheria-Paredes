using Microsoft.AspNetCore.Identity;

namespace Serralheria_Paredes.Models;

/// <summary>
/// Entidade de Perfil de Acesso (Role) do sistema.
/// </summary>
public class Perfil : IdentityRole<int>
{
    public Perfil() : base()
    {
    }

    public Perfil(string roleName, string? descricao = null) : base(roleName)
    {
        Descricao = descricao;
    }

    public string? Descricao { get; set; }
}
