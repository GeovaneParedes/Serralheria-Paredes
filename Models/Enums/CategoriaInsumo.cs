using System.ComponentModel.DataAnnotations;

namespace Serralheria_Paredes.Models.Enums;

public enum CategoriaInsumo
{
    [Display(Name = "Perfil de alumínio")]
    PerfilAluminio = 1,

    [Display(Name = "Tubo")]
    Tubo = 2,

    [Display(Name = "Cantoneira")]
    Cantoneira = 3,

    [Display(Name = "Chapa")]
    Chapa = 4,

    [Display(Name = "Ferragem")]
    Ferragem = 5,

    [Display(Name = "Consumível")]
    Consumivel = 6
}
