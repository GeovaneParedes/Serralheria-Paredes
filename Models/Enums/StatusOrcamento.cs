using System.ComponentModel.DataAnnotations;

namespace Serralheria_Paredes.Models.Enums;

public enum StatusOrcamento
{
    [Display(Name = "Rascunho")]
    Rascunho = 1,

    [Display(Name = "Enviado")]
    Enviado = 2,

    [Display(Name = "Aprovado")]
    Aprovado = 3,

    [Display(Name = "Recusado")]
    Recusado = 4,

    [Display(Name = "Expirado")]
    Expirado = 5
}
