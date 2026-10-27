using Serralheria_Paredes.Models.Common;
using Serralheria_Paredes.Models.Enums;

namespace Serralheria_Paredes.Models;

public class ItemOrcamento : BaseEntity
{
    public int OrcamentoId { get; set; }
    public Orcamento Orcamento { get; set; } = null!;
    public int? InsumoId { get; set; }
    public Insumo? Insumo { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public UnidadeMedida Unidade { get; set; }
    public decimal Quantidade { get; set; }
    public decimal ComprimentoUnitarioMetros { get; set; }
    public decimal PercentualPerda { get; set; }
    public decimal CustoUnitario { get; set; }
    public decimal Subtotal { get; set; }
}
