using Serralheria_Paredes.Models;

namespace Serralheria_Paredes.Repositories.Interfaces;

public interface IOrcamentoRepository : IRepository<Orcamento>
{
    Task<IReadOnlyList<Orcamento>> ObterTodosAsync(CancellationToken cancellationToken = default);
    Task<Orcamento?> ObterComItensAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> NumeroJaCadastradoAsync(string numero, CancellationToken cancellationToken = default);
}
