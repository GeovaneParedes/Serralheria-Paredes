using Serralheria_Paredes.Services.Common;
using Serralheria_Paredes.Common.Pagination;
using Serralheria_Paredes.ViewModels.Cadastros;

namespace Serralheria_Paredes.Services.Interfaces;

public interface IFornecedorService : IService
{
    Task<PagedResult<FornecedorViewModel>> ListarAsync(string? termo = null, int pagina = 1, int tamanho = Paginacao.TamanhoPadrao, CancellationToken cancellationToken = default);
    Task<OperationResult<FornecedorViewModel>> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<OperationResult<FornecedorFormViewModel>> ObterParaEdicaoAsync(int id, CancellationToken cancellationToken = default);
    Task<OperationResult<FornecedorViewModel>> CriarAsync(FornecedorFormViewModel model, CancellationToken cancellationToken = default);
    Task<OperationResult<FornecedorViewModel>> AtualizarAsync(int id, FornecedorFormViewModel model, CancellationToken cancellationToken = default);
    Task<OperationResult> AlternarStatusAsync(int id, bool ativo, CancellationToken cancellationToken = default);
}
