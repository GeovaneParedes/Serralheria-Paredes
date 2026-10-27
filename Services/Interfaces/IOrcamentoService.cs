using Serralheria_Paredes.Models.Enums;
using Serralheria_Paredes.Services.Common;

namespace Serralheria_Paredes.Services.Interfaces;

public interface IOrcamentoService : IService
{
    Task<IReadOnlyList<OrcamentoResumo>> ListarAsync(CancellationToken cancellationToken = default);
    Task<OperationResult<OrcamentoDetalhe>> ObterDetalhesAsync(int id, CancellationToken cancellationToken = default);
    Task<OperationResult<OrcamentoResumo>> CriarAsync(OrcamentoCadastro cadastro, CancellationToken cancellationToken = default);
    Task<OperationResult<OrcamentoResumo>> AprovarAsync(int id, CancellationToken cancellationToken = default);
    Task<OperationResult> AtualizarStatusOrdemAsync(int id, StatusOrdemServico status, CancellationToken cancellationToken = default);
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

public sealed record OrcamentoDetalhe(
    OrcamentoResumo Resumo,
    string? Descricao,
    decimal SubtotalMateriais,
    decimal ValorMaoDeObra,
    decimal PercentualMargem,
    IReadOnlyList<ItemOrcamentoResumo> Itens,
    OrdemServicoResumo? OrdemServico);

public sealed record ItemOrcamentoResumo(
    string Descricao,
    UnidadeMedida Unidade,
    decimal Quantidade,
    decimal ComprimentoUnitarioMetros,
    decimal PercentualPerda,
    decimal CustoUnitario,
    decimal Subtotal);

public sealed record OrdemServicoResumo(
    string Numero,
    StatusOrdemServico Status,
    DateTime? DataPrevistaEntrega,
    string? ObservacoesProducao);
