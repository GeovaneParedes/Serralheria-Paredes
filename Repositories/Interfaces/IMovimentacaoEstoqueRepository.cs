using Serralheria_Paredes.Models;

namespace Serralheria_Paredes.Repositories.Interfaces;

public interface IMovimentacaoEstoqueRepository : IRepository<MovimentacaoEstoque>
{
    Task<IReadOnlyList<MovimentacaoEstoque>> ObterHistoricoAsync(int? insumoId = null, CancellationToken cancellationToken = default);
}
