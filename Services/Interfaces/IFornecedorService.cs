using Serralheria_Paredes.Services.Common;
using Serralheria_Paredes.ViewModels.Cadastros;

namespace Serralheria_Paredes.Services.Interfaces;

public interface IFornecedorService : IService
{
    Task<IReadOnlyList<FornecedorViewModel>> ListarAsync(string? termo = null, CancellationToken cancellationToken = default);
    Task<OperationResult<FornecedorViewModel>> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<OperationResult<FornecedorFormViewModel>> ObterParaEdicaoAsync(int id, CancellationToken cancellationToken = default);
    Task<OperationResult<FornecedorViewModel>> CriarAsync(FornecedorFormViewModel model, CancellationToken cancellationToken = default);
    Task<OperationResult<FornecedorViewModel>> AtualizarAsync(int id, FornecedorFormViewModel model, CancellationToken cancellationToken = default);
    Task<OperationResult> AlternarStatusAsync(int id, bool ativo, CancellationToken cancellationToken = default);
}
