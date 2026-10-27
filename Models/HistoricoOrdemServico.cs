using Serralheria_Paredes.Models.Common;
using Serralheria_Paredes.Models.Enums;

namespace Serralheria_Paredes.Models;

public class HistoricoOrdemServico : BaseEntity
{
    public int OrdemServicoId { get; set; }
    public OrdemServico OrdemServico { get; set; } = null!;
    public StatusOrdemServico StatusAnterior { get; set; }
    public StatusOrdemServico StatusNovo { get; set; }
    public string? Observacao { get; set; }
    public int? UsuarioId { get; set; }
}
