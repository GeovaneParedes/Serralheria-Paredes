using Serralheria_Paredes.Models.Common;
using Serralheria_Paredes.Models.Enums;

namespace Serralheria_Paredes.Models;

public class Insumo : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public CategoriaInsumo Categoria { get; set; }
    public UnidadeMedida Unidade { get; set; }
    public decimal ComprimentoBarraMetros { get; set; }
    public decimal EstoqueAtual { get; set; }
    public decimal EstoqueMinimo { get; set; }
    public decimal CustoUnitario { get; set; }
    public int? FornecedorId { get; set; }
    public Fornecedor? Fornecedor { get; set; }
}
