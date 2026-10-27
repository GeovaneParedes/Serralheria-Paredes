using Serralheria_Paredes.Models;
using Serralheria_Paredes.Common.Pagination;

namespace Serralheria_Paredes.Repositories.Interfaces;

public interface IInsumoRepository : IRepository<Insumo>
{
    Task<PagedResult<Insumo>> ObterTodosComFiltroAsync(string? termo, int pagina, int tamanho, CancellationToken cancellationToken = default);
    Task<bool> CodigoJaCadastradoAsync(string codigo, int? ignorarId = null, CancellationToken cancellationToken = default);
}
