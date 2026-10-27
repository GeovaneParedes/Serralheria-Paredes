using Serralheria_Paredes.Models.Enums;
using Serralheria_Paredes.Services.Common;

namespace Serralheria_Paredes.Services.Interfaces;

public interface IOrcamentoService : IService
{
    Task<IReadOnlyList<OrcamentoResumo>> ListarAsync(CancellationToken cancellationToken = default);
    Task<OperationResult<OrcamentoResumo>> CriarAsync(OrcamentoCadastro cadastro, CancellationToken cancellationToken = default);
    Task<OperationResult<OrcamentoResumo>> AprovarAsync(int id, CancellationToken cancellationToken = default);
}

public sealed record OrcamentoItemCadastro(
    string Descricao,
    UnidadeMedida Unidade,
    decimal Quantidade,
    decimal ComprimentoUnitarioMetros,
    decimal PercentualPerda,
    decimal CustoUnitario,
    int? InsumoId = null);

public sealed record OrcamentoCadastro(
    int ClienteId,
    string Titulo,
    string? Descricao,
    DateTime ValidoAte,
    decimal ValorMaoDeObra,
    decimal PercentualMargem,
    IReadOnlyList<OrcamentoItemCadastro> Itens);

public sealed record OrcamentoResumo(
    int Id,
    string Numero,
    string Cliente,
    string Titulo,
    StatusOrcamento Status,
    decimal ValorTotal,
    DateTime ValidoAte,
    bool PossuiOrdemServico);
