namespace Serralheria_Paredes.Models.Constantes;

/// <summary>
/// Constantes com os perfis de acesso (Roles) do sistema.
/// </summary>
public static class PerfisConstantes
{
    public const string Administrador = "Administrador";
    public const string Serralheiro = "Serralheiro";
    public const string Atendente = "Atendente";

    public static readonly IReadOnlyList<string> Todos = new[]
    {
        Administrador,
        Serralheiro,
        Atendente
    };
}
