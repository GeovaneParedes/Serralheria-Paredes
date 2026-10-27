using System.ComponentModel.DataAnnotations;

namespace Serralheria_Paredes.Models.Enums;

public enum StatusOrdemServico
{
    [Display(Name = "Aguardando produção")]
    AguardandoProducao = 1,

    [Display(Name = "Em corte")]
    EmCorte = 2,

    [Display(Name = "Em montagem")]
    EmMontagem = 3,

    [Display(Name = "Em pintura")]
    EmPintura = 4,

    [Display(Name = "Em instalação")]
    EmInstalacao = 5,

    [Display(Name = "Concluída")]
    Concluida = 6,

    [Display(Name = "Cancelada")]
    Cancelada = 7
}
