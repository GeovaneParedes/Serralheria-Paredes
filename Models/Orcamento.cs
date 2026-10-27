using Serralheria_Paredes.Models.Common;
using Serralheria_Paredes.Models.Enums;

namespace Serralheria_Paredes.Models;

public class Orcamento : BaseEntity
{
    public string Numero { get; set; } = string.Empty;
    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime ValidoAte { get; set; }
    public StatusOrcamento Status { get; set; } = StatusOrcamento.Rascunho;
    public decimal SubtotalMateriais { get; set; }
    public decimal ValorMaoDeObra { get; set; }
    public decimal PercentualMargem { get; set; }
    public decimal ValorTotal { get; set; }
    public ICollection<ItemOrcamento> Itens { get; set; } = new List<ItemOrcamento>();
    public OrdemServico? OrdemServico { get; set; }
}
