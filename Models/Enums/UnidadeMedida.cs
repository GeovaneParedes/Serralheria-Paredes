using System.ComponentModel.DataAnnotations;

namespace Serralheria_Paredes.Models.Enums;

public enum UnidadeMedida
{
    [Display(Name = "Unidade")]
    Unidade = 1,

    [Display(Name = "Metro")]
    Metro = 2,

    [Display(Name = "Quilograma")]
    Quilograma = 3,

    [Display(Name = "Chapa")]
    Chapa = 4
}
