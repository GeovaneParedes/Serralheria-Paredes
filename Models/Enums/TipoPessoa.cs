using System.ComponentModel.DataAnnotations;

namespace Serralheria_Paredes.Models.Enums;

public enum TipoPessoa
{
    [Display(Name = "Pessoa Física")]
    Fisica = 1,

    [Display(Name = "Pessoa Jurídica")]
    Juridica = 2
}
