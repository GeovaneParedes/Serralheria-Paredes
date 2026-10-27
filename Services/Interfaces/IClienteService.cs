using Serralheria_Paredes.Services.Common;
using Serralheria_Paredes.Common.Pagination;
using Serralheria_Paredes.ViewModels.Cadastros;

namespace Serralheria_Paredes.Services.Interfaces;

public interface IClienteService : IService
{
    Task<PagedResult<ClienteViewModel>> ListarAsync(string? termo = null, int pagina = 1, int tamanho = Paginacao.TamanhoPadrao, CancellationToken cancellationToken = default);
    Task<OperationResult<ClienteViewModel>> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<OperationResult<ClienteFormViewModel>> ObterParaEdicaoAsync(int id, CancellationToken cancellationToken = default);
    Task<OperationResult<ClienteViewModel>> CriarAsync(ClienteFormViewModel model, CancellationToken cancellationToken = default);
    Task<OperationResult<ClienteViewModel>> AtualizarAsync(int id, ClienteFormViewModel model, CancellationToken cancellationToken = default);
    Task<OperationResult> AlternarStatusAsync(int id, bool ativo, CancellationToken cancellationToken = default);
}
