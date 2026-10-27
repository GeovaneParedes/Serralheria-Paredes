using Serralheria_Paredes.Services.Interfaces;

namespace Serralheria_Paredes.ViewModels.Orcamentos;

public class OrcamentoIndexViewModel
{
    public IReadOnlyList<OrcamentoResumo> Orcamentos { get; init; } = Array.Empty<OrcamentoResumo>();
}
