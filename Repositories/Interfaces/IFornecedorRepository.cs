using Serralheria_Paredes.Models;
using Serralheria_Paredes.Common.Pagination;

namespace Serralheria_Paredes.Repositories.Interfaces;

public interface IFornecedorRepository : IRepository<Fornecedor>
{
    Task<PagedResult<Fornecedor>> ObterTodosComFiltroAsync(string? termo, int pagina, int tamanho, CancellationToken cancellationToken = default);
    Task<bool> DocumentoJaCadastradoAsync(string cpfOuCnpj, int? ignorarId = null, CancellationToken cancellationToken = default);
}
