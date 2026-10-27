using Serralheria_Paredes.Models.Common;
using Serralheria_Paredes.Models.Enums;

namespace Serralheria_Paredes.Models;

/// <summary>
/// Entidade de Cliente da Serralheria (PF ou PJ).
/// Armazena dados cadastrais, contato e endereço para faturamento e entregas de obras/projetos.
/// </summary>
public class Cliente : BaseEntity
{
    public string NomeOuRazaoSocial { get; set; } = string.Empty;
    public string? NomeFantasia { get; set; }
    public TipoPessoa TipoPessoa { get; set; } = TipoPessoa.Fisica;
    public string CpfOuCnpj { get; set; } = string.Empty;
    public string? RgOuInscricaoEstadual { get; set; }

    public string Telefone { get; set; } = string.Empty;
    public string? Celular { get; set; }
    public string? Email { get; set; }

    // Endereço completo
    public string? EnderecoCep { get; set; }
    public string? EnderecoLogradouro { get; set; }
    public string? EnderecoNumero { get; set; }
    public string? EnderecoComplemento { get; set; }
    public string? EnderecoBairro { get; set; }
    public string? EnderecoCidade { get; set; }
    public string? EnderecoEstado { get; set; }

    public string? Observacoes { get; set; }
}
