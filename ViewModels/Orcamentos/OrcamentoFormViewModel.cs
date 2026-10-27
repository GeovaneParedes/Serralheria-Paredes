using System.ComponentModel.DataAnnotations;
using Serralheria_Paredes.Models.Enums;
using Serralheria_Paredes.Services.Interfaces;

namespace Serralheria_Paredes.ViewModels.Orcamentos;

public class OrcamentoFormViewModel
{
    [Required]
    [Display(Name = "Cliente")]
    public int ClienteId { get; set; }

    [Required, StringLength(150, MinimumLength = 3)]
    [Display(Name = "Título")]
    public string Titulo { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Descricao { get; set; }

    [Required, DataType(DataType.Date)]
    [Display(Name = "Válido até")]
    public DateTime ValidoAte { get; set; } = DateTime.Today.AddDays(15);

    [Range(typeof(decimal), "0", "999999999")]
    [Display(Name = "Mão de obra")]
    public decimal ValorMaoDeObra { get; set; }

    [Range(typeof(decimal), "0", "100")]
    [Display(Name = "Margem (%)")]
    public decimal PercentualMargem { get; set; } = 20;

    public List<OrcamentoItemFormViewModel> Itens { get; set; } = new() { new() };

    public OrcamentoCadastro ToCadastro() => new(
        ClienteId,
        Titulo,
        Descricao,
        ValidoAte,
        ValorMaoDeObra,
        PercentualMargem,
        Itens.Select(i => new OrcamentoItemCadastro(
            i.Descricao,
            i.Unidade,
            i.Quantidade,
            i.ComprimentoUnitarioMetros,
            i.PercentualPerda,
            i.CustoUnitario,
            i.InsumoId)).ToList());
}

public class OrcamentoItemFormViewModel
{
    [Required]
    public string Descricao { get; set; } = string.Empty;

    public UnidadeMedida Unidade { get; set; } = UnidadeMedida.Metro;

    [Range(typeof(decimal), "0.001", "999999999")]
    public decimal Quantidade { get; set; }

    [Range(typeof(decimal), "0", "999999999")]
    [Display(Name = "Comprimento unitário (m)")]
    public decimal ComprimentoUnitarioMetros { get; set; } = 1;

    [Range(typeof(decimal), "0", "100")]
    [Display(Name = "Perda (%)")]
    public decimal PercentualPerda { get; set; }

    [Range(typeof(decimal), "0", "999999999")]
    [Display(Name = "Custo unitário")]
    public decimal CustoUnitario { get; set; }

    public int? InsumoId { get; set; }
}
