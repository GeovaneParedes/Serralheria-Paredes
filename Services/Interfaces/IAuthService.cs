using Serralheria_Paredes.Services.Common;
using Serralheria_Paredes.ViewModels.Auth;

namespace Serralheria_Paredes.Services.Interfaces;

/// <summary>
/// Contrato do serviço de autenticação, autorização e gerenciamento de usuários.
/// </summary>
public interface IAuthService : IService
{
    Task<OperationResult<UsuarioViewModel>> LoginAsync(LoginViewModel model, CancellationToken cancellationToken = default);
    Task LogoutAsync();
    Task<OperationResult<UsuarioViewModel>> RegistrarUsuarioAsync(NovoUsuarioViewModel model, CancellationToken cancellationToken = default);
    Task<OperationResult<UsuarioViewModel>> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UsuarioViewModel>> ListarUsuariosAsync(CancellationToken cancellationToken = default);
    Task<OperationResult> AlterarStatusUsuarioAsync(int id, bool ativo, CancellationToken cancellationToken = default);
}
