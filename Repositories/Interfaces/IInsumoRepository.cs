using Serralheria_Paredes.Models;

namespace Serralheria_Paredes.Repositories.Interfaces;

public interface IInsumoRepository : IRepository<Insumo>
{
    Task<IReadOnlyList<Insumo>> ObterTodosComFiltroAsync(string? termo, CancellationToken cancellationToken = default);
    Task<bool> CodigoJaCadastradoAsync(string codigo, int? ignorarId = null, CancellationToken cancellationToken = default);
}
