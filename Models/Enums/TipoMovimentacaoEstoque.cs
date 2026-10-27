using System.ComponentModel.DataAnnotations;

namespace Serralheria_Paredes.Models.Enums;

public enum TipoMovimentacaoEstoque
{
    [Display(Name = "Entrada")]
    Entrada = 1,

    [Display(Name = "Saída")]
    Saida = 2,

    [Display(Name = "Ajuste")]
    Ajuste = 3
}
