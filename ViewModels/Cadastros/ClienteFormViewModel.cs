using System.ComponentModel.DataAnnotations;
using Serralheria_Paredes.Models.Enums;

namespace Serralheria_Paredes.ViewModels.Cadastros;

public class ClienteFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O Tipo de Pessoa (Física ou Jurídica) é obrigatório.")]
    [Display(Name = "Tipo de Pessoa")]
    public TipoPessoa TipoPessoa { get; set; } = TipoPessoa.Fisica;

    [Required(ErrorMessage = "O Nome ou Razão Social é obrigatório.")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "O Nome deve ter entre 3 e 150 caracteres.")]
    [Display(Name = "Nome Completo / Razão Social")]
    public string NomeOuRazaoSocial { get; set; } = string.Empty;

    [StringLength(150)]
    [Display(Name = "Nome Fantasia (opcional)")]
    public string? NomeFantasia { get; set; }

    [Required(ErrorMessage = "O CPF ou CNPJ é obrigatório.")]
    [Display(Name = "CPF ou CNPJ")]
    public string CpfOuCnpj { get; set; } = string.Empty;

    [StringLength(25)]
    [Display(Name = "RG / Inscrição Estadual (opcional)")]
    public string? RgOuInscricaoEstadual { get; set; }

    [Required(ErrorMessage = "O telefone de contato é obrigatório.")]
    [StringLength(20)]
    [Display(Name = "Telefone Fixo / Comercial")]
    public string Telefone { get; set; } = string.Empty;

    [StringLength(20)]
    [Display(Name = "Celular / WhatsApp")]
    public string? Celular { get; set; }

    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    [StringLength(100)]
    [Display(Name = "E-mail de Contato")]
    public string? Email { get; set; }

    // Endereço
    [StringLength(10)]
    [Display(Name = "CEP")]
    public string? EnderecoCep { get; set; }

    [StringLength(150)]
    [Display(Name = "Logradouro (Rua, Av.)")]
    public string? EnderecoLogradouro { get; set; }

    [StringLength(20)]
    [Display(Name = "Número")]
    public string? EnderecoNumero { get; set; }

    [StringLength(100)]
    [Display(Name = "Complemento")]
    public string? EnderecoComplemento { get; set; }

    [StringLength(80)]
    [Display(Name = "Bairro")]
    public string? EnderecoBairro { get; set; }

    [StringLength(80)]
    [Display(Name = "Cidade")]
    public string? EnderecoCidade { get; set; }

    [StringLength(2, MinimumLength = 2, ErrorMessage = "A UF deve conter 2 letras (ex: SP, MG, RJ).")]
    [Display(Name = "Estado (UF)")]
    public string? EnderecoEstado { get; set; }

    [StringLength(500)]
    [Display(Name = "Observações Gerais")]
    public string? Observacoes { get; set; }

    public bool IsActive { get; set; } = true;
}
