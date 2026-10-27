using Serralheria_Paredes.Models;

namespace Serralheria_Paredes.Repositories.Interfaces;

public interface IClienteRepository : IRepository<Cliente>
{
    Task<IReadOnlyList<Cliente>> ObterTodosComFiltroAsync(string? termo, CancellationToken cancellationToken = default);
    Task<bool> DocumentoJaCadastradoAsync(string cpfOuCnpj, int? ignorarId = null, CancellationToken cancellationToken = default);
}
