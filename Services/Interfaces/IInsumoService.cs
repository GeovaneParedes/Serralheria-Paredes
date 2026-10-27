using Serralheria_Paredes.Models.Enums;
using Serralheria_Paredes.Services.Common;
using Serralheria_Paredes.Common.Pagination;

namespace Serralheria_Paredes.Services.Interfaces;

public interface IInsumoService : IService
{
    Task<PagedResult<InsumoResumo>> ListarAsync(string? termo = null, int pagina = 1, int tamanho = Paginacao.TamanhoPadrao, CancellationToken cancellationToken = default);
    Task<OperationResult<InsumoResumo>> CriarAsync(InsumoCadastro cadastro, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MovimentacaoResumo>> HistoricoAsync(int? insumoId = null, CancellationToken cancellationToken = default);
    Task<OperationResult> MovimentarAsync(int id, TipoMovimentacaoEstoque tipo, decimal quantidade, string motivo, int? usuarioId = null, CancellationToken cancellationToken = default);
    Task<OperationResult> ConsumirParaOrdemAsync(int ordemServicoId, int insumoId, decimal quantidade, int? usuarioId = null, CancellationToken cancellationToken = default);
}

public sealed record InsumoCadastro(
    string Codigo,
    string Descricao,
    CategoriaInsumo Categoria,
    UnidadeMedida Unidade,
    decimal ComprimentoBarraMetros,
    decimal EstoqueMinimo,
    decimal CustoUnitario,
    int? FornecedorId);

public sealed record InsumoResumo(
    int Id,
    string Codigo,
    string Descricao,
    CategoriaInsumo Categoria,
    UnidadeMedida Unidade,
    decimal EstoqueAtual,
    decimal EstoqueMinimo,
    decimal CustoUnitario,
    bool EstoqueAbaixoDoMinimo,
    string? Fornecedor);

public sealed record MovimentacaoResumo(
    int Id,
    int InsumoId,
    string Insumo,
    TipoMovimentacaoEstoque Tipo,
    decimal Quantidade,
    decimal SaldoAnterior,
    decimal SaldoPosterior,
    string Motivo,
    DateTime CriadaEm);
