using System.ComponentModel.DataAnnotations;
using Serralheria_Paredes.Models.Enums;

namespace Serralheria_Paredes.ViewModels.Cadastros;

public class FornecedorFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O Tipo de Pessoa é obrigatório.")]
    [Display(Name = "Tipo de Pessoa")]
    public TipoPessoa TipoPessoa { get; set; } = TipoPessoa.Juridica;

    [Required(ErrorMessage = "A Razão Social é obrigatória.")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "A Razão Social deve ter entre 3 e 150 caracteres.")]
    [Display(Name = "Razão Social / Nome")]
    public string RazaoSocial { get; set; } = string.Empty;

    [StringLength(150)]
    [Display(Name = "Nome Fantasia (opcional)")]
    public string? NomeFantasia { get; set; }

    [Required(ErrorMessage = "O CNPJ ou CPF é obrigatório.")]
    [Display(Name = "CNPJ ou CPF")]
    public string CpfOuCnpj { get; set; } = string.Empty;

    [StringLength(25)]
    [Display(Name = "Inscrição Estadual (opcional)")]
    public string? InscricaoEstadual { get; set; }

    [Required(ErrorMessage = "O telefone de contato é obrigatório.")]
    [StringLength(20)]
    [Display(Name = "Telefone Comercial")]
    public string Telefone { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    [StringLength(100)]
    [Display(Name = "E-mail de Contato / Cotações")]
    public string? Email { get; set; }

    [StringLength(80)]
    [Display(Name = "Nome do Vendedor / Representante")]
    public string? ContatoResponsavel { get; set; }

    // Endereço
    [StringLength(10)]
    [Display(Name = "CEP")]
    public string? EnderecoCep { get; set; }

    [StringLength(150)]
    [Display(Name = "Logradouro")]
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

    [StringLength(2, MinimumLength = 2, ErrorMessage = "A UF deve conter 2 letras.")]
    [Display(Name = "Estado (UF)")]
    public string? EnderecoEstado { get; set; }

    [StringLength(500)]
    [Display(Name = "Observações / Linha de Produtos")]
    public string? Observacoes { get; set; }

    public bool IsActive { get; set; } = true;
}
