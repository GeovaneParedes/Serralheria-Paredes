using Serralheria_Paredes.Models.Common;
using Serralheria_Paredes.Models.Enums;

namespace Serralheria_Paredes.Models;

/// <summary>
/// Entidade de Fornecedor da Serralheria (Perfis, Tubos, Chapas, Ferragens, Tintas).
/// </summary>
public class Fornecedor : BaseEntity
{
    public string RazaoSocial { get; set; } = string.Empty;
    public string? NomeFantasia { get; set; }
    public TipoPessoa TipoPessoa { get; set; } = TipoPessoa.Juridica;
    public string CpfOuCnpj { get; set; } = string.Empty;
    public string? InscricaoEstadual { get; set; }

    public string Telefone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? ContatoResponsavel { get; set; }

    // Endereço
    public string? EnderecoCep { get; set; }
    public string? EnderecoLogradouro { get; set; }
    public string? EnderecoNumero { get; set; }
    public string? EnderecoComplemento { get; set; }
    public string? EnderecoBairro { get; set; }
    public string? EnderecoCidade { get; set; }
    public string? EnderecoEstado { get; set; }

    public string? Observacoes { get; set; }
}
