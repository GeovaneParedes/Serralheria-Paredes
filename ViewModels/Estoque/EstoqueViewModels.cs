using System.ComponentModel.DataAnnotations;
using Serralheria_Paredes.Models.Enums;
using Serralheria_Paredes.Services.Interfaces;

namespace Serralheria_Paredes.ViewModels.Estoque;

public class EstoqueIndexViewModel
{
    public IReadOnlyList<InsumoResumo> Insumos { get; init; } = Array.Empty<InsumoResumo>();
    public string? Termo { get; init; }
}

public class MovimentacaoEstoqueFormViewModel
{
    public int InsumoId { get; set; }

    [Required(ErrorMessage = "O tipo de movimentação é obrigatório.")]
    [Display(Name = "Tipo")]
    public TipoMovimentacaoEstoque Tipo { get; set; } = TipoMovimentacaoEstoque.Entrada;

    [Range(typeof(decimal), "0.001", "999999999", ErrorMessage = "Informe uma quantidade maior que zero.")]
    [Display(Name = "Quantidade")]
    public decimal Quantidade { get; set; }

    [Required(ErrorMessage = "O motivo é obrigatório.")]
    [StringLength(250, MinimumLength = 3, ErrorMessage = "O motivo deve ter entre 3 e 250 caracteres.")]
    [Display(Name = "Motivo")]
    public string Motivo { get; set; } = string.Empty;

    public string InsumoDescricao { get; set; } = string.Empty;
    public decimal SaldoAtual { get; set; }
    public UnidadeMedida Unidade { get; set; }
}
