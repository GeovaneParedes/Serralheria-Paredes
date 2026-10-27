using Serralheria_Paredes.Common.Validation;
using Serralheria_Paredes.Models.Enums;

namespace Serralheria_Paredes.ViewModels.Cadastros;

public class FornecedorViewModel
{
    public int Id { get; set; }
    public string RazaoSocial { get; set; } = string.Empty;
    public string? NomeFantasia { get; set; }
    public TipoPessoa TipoPessoa { get; set; }
    public string CpfOuCnpj { get; set; } = string.Empty;
    public string CpfOuCnpjFormatado => DocumentosUtils.Formatar(CpfOuCnpj);
    public string? InscricaoEstadual { get; set; }
    public string Telefone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? ContatoResponsavel { get; set; }

    public string? EnderecoCidade { get; set; }
    public string? EnderecoEstado { get; set; }
    public string Localizacao => !string.IsNullOrEmpty(EnderecoCidade) ? $"{EnderecoCidade}/{EnderecoEstado}" : "-";

    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
