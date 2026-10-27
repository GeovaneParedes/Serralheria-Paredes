using Serralheria_Paredes.Models.Common;
using Serralheria_Paredes.Models.Enums;

namespace Serralheria_Paredes.Models;

public class OrdemServico : BaseEntity
{
    public string Numero { get; set; } = string.Empty;
    public int OrcamentoId { get; set; }
    public Orcamento Orcamento { get; set; } = null!;
    public StatusOrdemServico Status { get; set; } = StatusOrdemServico.AguardandoProducao;
    public DateTime? DataPrevistaEntrega { get; set; }
    public string? ObservacoesProducao { get; set; }
    public ICollection<HistoricoOrdemServico> Historico { get; set; } = new List<HistoricoOrdemServico>();
    public ICollection<MovimentacaoEstoque> MovimentacoesEstoque { get; set; } = new List<MovimentacaoEstoque>();
}
