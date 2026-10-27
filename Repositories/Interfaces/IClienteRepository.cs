using Serralheria_Paredes.Models;
using Serralheria_Paredes.Common.Pagination;

namespace Serralheria_Paredes.Repositories.Interfaces;

public interface IClienteRepository : IRepository<Cliente>
{
    Task<PagedResult<Cliente>> ObterTodosComFiltroAsync(string? termo, int pagina, int tamanho, CancellationToken cancellationToken = default);
    Task<bool> DocumentoJaCadastradoAsync(string cpfOuCnpj, int? ignorarId = null, CancellationToken cancellationToken = default);
}
