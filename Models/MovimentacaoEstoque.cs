using Serralheria_Paredes.Models.Common;
using Serralheria_Paredes.Models.Enums;

namespace Serralheria_Paredes.Models;

public class MovimentacaoEstoque : BaseEntity
{
    public int InsumoId { get; set; }
    public Insumo Insumo { get; set; } = null!;
    public TipoMovimentacaoEstoque Tipo { get; set; }
    public decimal Quantidade { get; set; }
    public decimal SaldoAnterior { get; set; }
    public decimal SaldoPosterior { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public int? UsuarioId { get; set; }
}
