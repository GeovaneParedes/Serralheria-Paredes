using Serralheria_Paredes.Models.Enums;
using Serralheria_Paredes.Services.Common;

namespace Serralheria_Paredes.Services.Interfaces;

public interface IInsumoService : IService
{
    Task<IReadOnlyList<InsumoResumo>> ListarAsync(string? termo = null, CancellationToken cancellationToken = default);
    Task<OperationResult<InsumoResumo>> CriarAsync(InsumoCadastro cadastro, CancellationToken cancellationToken = default);
    Task<OperationResult> AjustarEstoqueAsync(int id, decimal quantidade, CancellationToken cancellationToken = default);
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
